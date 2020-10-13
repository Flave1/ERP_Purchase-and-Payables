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
    public class UploadServiceTermsCommand : IRequest<FileUploadRespObj>
    {
        public class UploadServiceTermsCommandHandler : IRequestHandler<UploadServiceTermsCommand, FileUploadRespObj>
        {
            private readonly IHttpContextAccessor _accessor;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly ISupplierRepository _repo;
            public UploadServiceTermsCommandHandler(
                IHttpContextAccessor httpContextAccessor, 
                IIdentityServerRequest identityServerRequest,
                ISupplierRepository supplierRepository) 
            {
                _serverRequest = identityServerRequest;
                _accessor = httpContextAccessor;
                _repo = supplierRepository;
            }
            public async Task<FileUploadRespObj> Handle(UploadServiceTermsCommand request, CancellationToken cancellationToken)
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
                     
                    var uploadedRecord = new List<ServiceTermObj>();
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
                                    var item = new ServiceTermObj
                                    {
                                        ExcelLineNumber = i,
                                        Header = workSheet.Cells[i, 1]?.Value != null ? workSheet.Cells[i, 1]?.Value.ToString() : string.Empty,
                                        Content = workSheet.Cells[i, 2]?.Value != null ? workSheet.Cells[i, 2]?.Value.ToString() : string.Empty,
                                };
                                    uploadedRecord.Add(item);
                                }
                            }
                        }

                    }


                    var ItemList = await _repo.GetAllServiceTermsAsync();
                    cor_serviceterms itemFrmRepo = new cor_serviceterms();
                    if (uploadedRecord.Count > 0)
                    {
                        foreach (var item in uploadedRecord)
                        {
                            if (string.IsNullOrEmpty(item.Content))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $" Empty Content Expected Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (string.IsNullOrEmpty(item.Header))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $" Empty Header Expected Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }

                            itemFrmRepo = ItemList.FirstOrDefault(c => c.Header.ToLower().Trim() == item.Header.ToLower().Trim());

                            if (itemFrmRepo != null)
                            {
                                itemFrmRepo.Header = item?.Header; 
                                itemFrmRepo.Content = item?.Content; 

                                await _repo.AddUpdateSeviceTermAsync(itemFrmRepo);
                            }
                            else
                            {
                                var newItem = new cor_serviceterms
                                {
                                    Header = item?.Header, 
                                    Content = item?.Content,
                                };
                                await _repo.AddUpdateSeviceTermAsync(newItem);
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
