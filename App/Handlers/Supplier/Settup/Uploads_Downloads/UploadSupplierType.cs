using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.RequestResponse.Supplier;
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
    public class UploadSupplierTypeCommand : IRequest<FileUploadRespObj>
    {
        public class UploadSupplierTypeCommandHandler : IRequestHandler<UploadSupplierTypeCommand, FileUploadRespObj>
        {
            private readonly IHttpContextAccessor _accessor;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly ISupplierRepository _repo;
            public UploadSupplierTypeCommandHandler(
                IHttpContextAccessor httpContextAccessor, 
                IIdentityServerRequest identityServerRequest,
                ISupplierRepository supplierRepository) 
            {
                _serverRequest = identityServerRequest;
                _accessor = httpContextAccessor;
                _repo = supplierRepository;
            }
            public async Task<FileUploadRespObj> Handle(UploadSupplierTypeCommand request, CancellationToken cancellationToken)
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
                    var user = await _serverRequest.UserDataAsync();
                     
                    var uploadedRecord = new List<SupplierTypeObj>();
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

                                if (totalColumns != 2)
                                {
                                    apiResponse.Status.Message.FriendlyMessage = "2 Columns Expected";
                                    return apiResponse;
                                }

                                for (int i = 2; i <= totalRows; i++)
                                {
                                    var item = new SupplierTypeObj
                                    {
                                        ExcelLineNumber = i,
                                        SupplierTypeName = workSheet.Cells[i, 1]?.Value != null ? workSheet.Cells[i, 1]?.Value.ToString() : string.Empty,
                                        TaxApplicableName = workSheet.Cells[i, 2]?.Value != null ? workSheet.Cells[i, 2]?.Value.ToString() : string.Empty,
                                    };
                                    uploadedRecord.Add(item);
                                }
                            }
                        } 
                    }


                    var ItemList = await _repo.GetAllSupplierTypeAsync();
                    var TaxList = await _repo.GetAllTaxSetupAsync();
                    if (uploadedRecord.Count > 0)
                    {
                        var listOftaxt = new List<int>();

                        foreach (var item in uploadedRecord)
                        {
                            if (string.IsNullOrEmpty(item.TaxApplicableName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"No Tax Applicable found Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            else
                            {
                                var taxNames = item.TaxApplicableName.Trim().ToLower().Split(','); 
                                foreach(var tx in taxNames)
                                { 
                                    var taxes = TaxList.FirstOrDefault(a => taxNames.Contains(a.TaxName.Trim().ToLower()));
                                    if(taxes == null)
                                    {
                                        apiResponse.Status.Message.FriendlyMessage = $"Unidentified Tax name Detected on line {item.ExcelLineNumber}";
                                        return apiResponse;
                                    }
                                    listOftaxt.Add(taxes.TaxSetupId);
                                } 
                            }
                            if (string.IsNullOrEmpty(item.SupplierTypeName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty Supplier Type Name Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }

                            var itemFrmRepo = ItemList.FirstOrDefault(c => c.SupplierTypeName.ToLower().Trim() == item.SupplierTypeName.ToLower().Trim());

                            if (itemFrmRepo != null)
                            {
                                itemFrmRepo.SupplierTypeName = item?.SupplierTypeName;  
                                itemFrmRepo.TaxApplicable = string.Join(',', listOftaxt);  
                                await _repo.AddUpdateSupplierTypeAsync(itemFrmRepo);
                            }
                            else
                            {
                                var newItem = new cor_suppliertype
                                {
                                    SupplierTypeName = item?.SupplierTypeName,
                                    TaxApplicable = string.Join(',', listOftaxt),
                                };
                                await _repo.AddUpdateSupplierTypeAsync(newItem);
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
