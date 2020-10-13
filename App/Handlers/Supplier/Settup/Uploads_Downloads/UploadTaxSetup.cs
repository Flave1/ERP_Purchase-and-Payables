using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Puchase_and_payables.Contracts.GeneralExtension;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Settup
{
    public class UploadTaxSetupCommand : IRequest<FileUploadRespObj>
    {
        public class UploadTaxSetupCommandHandler : IRequestHandler<UploadTaxSetupCommand, FileUploadRespObj>
        {
            private readonly IHttpContextAccessor _accessor;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly ISupplierRepository _repo;
            public UploadTaxSetupCommandHandler(
                IHttpContextAccessor httpContextAccessor, 
                IIdentityServerRequest identityServerRequest,
                ISupplierRepository supplierRepository) 
            {
                _serverRequest = identityServerRequest;
                _accessor = httpContextAccessor;
                _repo = supplierRepository;
            }
            public async Task<FileUploadRespObj> Handle(UploadTaxSetupCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var apiResponse = new FileUploadRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };

                    var files = _accessor.HttpContext.Request.Form.Files;

                    var byteList = new List<byte[]>();
                    foreach (var fileBit in files)
                    {
                        if (fileBit.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                await fileBit.CopyToAsync(ms);
                                byteList.Add(ms.ToArray());
                            }
                        }

                    }

                    var userId = _accessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
                    var user = await _serverRequest.UserDataAsync();
                     
                    var uploadedRecord = new List<TaxsetupObj>();
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    if (byteList.Count() > 0)
                    {
                        foreach (var byteItem in byteList)
                        {
                            using (MemoryStream stream = new MemoryStream(byteItem))
                            using (ExcelPackage excelPackage = new ExcelPackage(stream))
                            { 
                                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                                int totalRows = workSheet.Dimension.Rows;
                                int totalColumns = workSheet.Dimension.Columns;

                                if (totalColumns != 4)
                                {
                                    apiResponse.Status.Message.FriendlyMessage = "4 Columns Expected";
                                    return apiResponse;
                                }

                                for (int i = 2; i <= totalRows; i++)
                                {
                                    var item = new TaxsetupObj
                                    {
                                        ExcelLineNumber = i,
                                        TaxName = workSheet.Cells[i, 1]?.Value != null ? workSheet.Cells[i, 1]?.Value.ToString() : string.Empty,
                                        SubGlName = workSheet.Cells[i, 2]?.Value != null ? workSheet.Cells[i, 2]?.Value.ToString() : string.Empty,
                                        Percentage = workSheet.Cells[i, 3]?.Value != null ? Convert.ToDouble(workSheet.Cells[i, 3]?.Value.ToString()) :0,
                                        Type = workSheet.Cells[i, 4]?.Value != null ? workSheet.Cells[i, 4]?.Value.ToString() : string.Empty,
                                    };
                                    uploadedRecord.Add(item);
                                }
                            }
                        }

                    }


                    var ItemList = await _repo.GetAllTaxSetupAsync();
                    cor_taxsetup itemFrmRepo = new cor_taxsetup();
                    if (uploadedRecord.Count > 0)
                    {
                        foreach (var item in uploadedRecord)
                        {
                            if (string.IsNullOrEmpty(item.TaxName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty tax name detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (string.IsNullOrEmpty(item.SubGlName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Sub Gl name is empty detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            //if (item.Percentage < 1)
                            //{
                            //    apiResponse.Status.Message.FriendlyMessage = $"Sub Gl name is empty detected on line {item.ExcelLineNumber}";
                            //    return apiResponse;
                            //}
                            if (string.IsNullOrEmpty(item.Type))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Type is empty detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            itemFrmRepo = ItemList.FirstOrDefault(c => c.TaxName.ToLower().Trim() == item?.TaxName.ToLower().Trim());
                            if (itemFrmRepo != null)
                            {
                                itemFrmRepo.TaxName = item?.TaxName;
                                itemFrmRepo.TaxSetupId = itemFrmRepo.TaxSetupId;
                                itemFrmRepo.Percentage = item.Percentage;
                                itemFrmRepo.SubGL = itemFrmRepo.SubGL;
                                itemFrmRepo.Type = item?.Type; 

                                await _repo.AddUpdateTaxSetupAsync(itemFrmRepo);
                            }
                            else
                            {
                                var newItem = new cor_taxsetup();
                                newItem.TaxName = item?.TaxName;
                                newItem.Percentage = item.Percentage;
                                newItem.SubGL = item.SubGL;
                                newItem.Type = item?.Type;
                                await _repo.AddUpdateTaxSetupAsync(newItem);
                            }
                        }
                    }
                    apiResponse.Status.IsSuccessful = true;
                    apiResponse.Status.Message.FriendlyMessage = "Successful";
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
