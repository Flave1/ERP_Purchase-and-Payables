using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using Puchase_and_payables.Contracts.GeneralExtension;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Uploads_Downloads
{
    public class UploadSupplierInformationCommand : IRequest<FileUploadRespObj>
    {
        public class UploadSupplierInformationCommandHandler : IRequestHandler<UploadSupplierInformationCommand, FileUploadRespObj>
        {
            private readonly ISupplierRepository _repo;
            private readonly IHttpContextAccessor _accessor;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IIdentityServerRequest _serverRequest;
            public UploadSupplierInformationCommandHandler(
                UserManager<ApplicationUser> userManager,
                ISupplierRepository supplierRepository,
                IHttpContextAccessor httpContextAccessor,
                IIdentityServerRequest identityServerRequest)
            {
                _repo = supplierRepository;
                _userManager = userManager;
                _serverRequest = identityServerRequest;
                _accessor = httpContextAccessor;
            }
            public async Task<FileUploadRespObj> Handle(UploadSupplierInformationCommand request, CancellationToken cancellationToken)
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

                    var uploadedRecord = new List<SupplierObj>();
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

                                if (totalColumns != 14)
                                {
                                    apiResponse.Status.Message.FriendlyMessage = "14 Columns Expected";
                                    return apiResponse;
                                }
                                    var item = new SupplierObj();
                                for (int i = 2; i <= totalRows; i++)
                                {
                                    item.ExcelLineNumber = i;
                                    item.SupplierTypeName = workSheet.Cells[i, 1]?.Value != null ? workSheet.Cells[i, 1]?.Value.ToString() : string.Empty;
                                    item.Name = workSheet.Cells[i, 2]?.Value != null ? workSheet.Cells[i, 2]?.Value.ToString() : string.Empty;
                                    item.Address = workSheet.Cells[i, 3]?.Value != null ? workSheet.Cells[i, 3]?.Value.ToString() : string.Empty;
                                    item.PhoneNo = workSheet.Cells[i, 4]?.Value != null ? workSheet.Cells[i, 4]?.Value.ToString() : string.Empty;
                                    item.Email = workSheet.Cells[i, 5]?.Value != null ? workSheet.Cells[i, 5]?.Value.ToString() : string.Empty;
                                    item.RegistrationNo = workSheet.Cells[i, 6]?.Value != null ? workSheet.Cells[i, 6]?.Value.ToString() : string.Empty;
                                    item.SupplierNumber = workSheet.Cells[i, 7]?.Value != null ? workSheet.Cells[i, 7]?.Value.ToString() : string.Empty;
                                    item.CountryName = workSheet.Cells[i, 8]?.Value != null ? workSheet.Cells[i, 8]?.Value.ToString() : string.Empty;
                                    item.StatusName = workSheet.Cells[i, 9]?.Value != null ? workSheet.Cells[i, 9]?.Value.ToString() : string.Empty;
                                    item.Website = workSheet.Cells[i, 10]?.Value != null ? workSheet.Cells[i, 10]?.Value.ToString() : string.Empty;
                                    item.PostalAddress = workSheet.Cells[i, 11]?.Value != null ? workSheet.Cells[i, 11]?.Value.ToString() : string.Empty;
                                    item.TaxIDorVATID = workSheet.Cells[i, 12]?.Value != null ? workSheet.Cells[i, 12]?.Value.ToString() : string.Empty;
                                    item.WorkPermitName = workSheet.Cells[i, 13]?.Value != null ? workSheet.Cells[i, 13]?.Value.ToString() : string.Empty;
                                    item.ParticularsName = workSheet.Cells[i, 14]?.Value != null ? workSheet.Cells[i, 14]?.Value.ToString() : string.Empty; 
                                    
                                    uploadedRecord.Add(item);
                                }
                            }
                        } 
                    } 
                    var ItemList = await _repo.GetAllSupplierAsync();
                    var _SupplierType = await _repo.GetAllSupplierTypeAsync();
                    var _country = await _serverRequest.GetAllCountryAsync();
                    cor_supplier itemFrmRepo = new cor_supplier();
                    if (uploadedRecord.Count > 0)
                    {
                        foreach (var item in uploadedRecord)
                        { 
                            if (string.IsNullOrEmpty(item.Name))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty Supplier name detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            else
                            {
                                if (item.Name.Split(" ").Length == 1)
                                {
                                    apiResponse.Status.Message.FriendlyMessage = $"Invalid Supplier name detected on line {item.ExcelLineNumber}";
                                    return apiResponse;
                                }
                            }
                            itemFrmRepo = ItemList.FirstOrDefault(c => c.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                            if (string.IsNullOrEmpty(item.SupplierTypeName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty Supplier type detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            else
                            {
                                var type = _SupplierType.FirstOrDefault(w => w.SupplierTypeName.Trim().ToLower() == item.SupplierTypeName.Trim().ToLower());
                                if(type == null)
                                {
                                    apiResponse.Status.Message.FriendlyMessage = $"Undentified Supplier type detected on line {item.ExcelLineNumber}";
                                    return apiResponse;
                                }
                                item.SupplierTypeId = type.SupplierTypeId;
                            }
                            if (string.IsNullOrEmpty(item.Email))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Email Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            else
                            {
                                try
                                {
                                    MailAddress m = new MailAddress(item.Email);
                                }
                                catch (FormatException)
                                {
                                    apiResponse.Status.Message.FriendlyMessage = $"Invalid Email Detected on line {item.ExcelLineNumber}";
                                    return apiResponse;
                                }
                                var userEmailExist = _userManager.Users?.Where(w => w.Email.Trim().ToLower() == item.Email.Trim()).ToList();
                                if(userEmailExist.Count() > 1)
                                {
                                    apiResponse.Status.Message.FriendlyMessage = $"Duplicate Supplier Email Detected on line {item.ExcelLineNumber}";
                                    return apiResponse;
                                } 
                            }

                            if (string.IsNullOrEmpty(item.Address))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Address Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (string.IsNullOrEmpty(item.RegistrationNo))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Reg No Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (string.IsNullOrEmpty(item.StatusName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Status name Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            else
                            {
                                if(!Enum.IsDefined(typeof(ApprovalStatus), item.StatusName))
                                {
                                    apiResponse.Status.Message.FriendlyMessage = $"Invalid Status Detected on line {item.ExcelLineNumber}";
                                    return apiResponse;
                                }
                            }
                            if (string.IsNullOrEmpty(item.SupplierNumber) && item.StatusName.Trim().ToLower() != "pending")
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Status field of a Supplier without Supplier Number should be pending Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (!string.IsNullOrEmpty(item.SupplierNumber) && item.StatusName.Trim().ToLower() == "pending")
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Status field of a Supplier with Supplier Number should not be pending Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (!string.IsNullOrEmpty(item.SupplierNumber))
                            {
                                var supNumExist = await _repo.GetAllSupplierAsync();
                                if(supNumExist.Count() > 0)
                                {
                                    if (supNumExist?.Where(q => q.SupplierNumber.Trim().ToLower() == item.SupplierNumber.Trim().ToLower()).Count() > 1)
                                    {
                                        apiResponse.Status.Message.FriendlyMessage = $"Supplier with same supplier number already exist Detected on line {item.ExcelLineNumber}";
                                        return apiResponse;
                                    }
                                }
                                
                            }

                            if (string.IsNullOrEmpty(item.Address))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Email Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (string.IsNullOrEmpty(item.PhoneNo))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on PhoneNo Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            
                            if (string.IsNullOrEmpty(item.Website))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Wwebsite Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (string.IsNullOrEmpty(item.PostalAddress))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on PostalAddress Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (string.IsNullOrEmpty(item.TaxIDorVATID))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Tax Vat Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            if (string.IsNullOrEmpty(item.WorkPermitName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Work permit  Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            else
                            {
                                item.HaveWorkPrintPermit = item.WorkPermitName.Trim().ToLower() == "Yes".Trim().ToLower() ? true : false;
                            }
                            if (string.IsNullOrEmpty(item.ParticularsName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Particulars Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            else
                            {
                                item.Particulars = item.ParticularsName.Trim().ToLower() == "Corporate".ToLower().Trim() ? 2 : 1;
                            }
                            if (string.IsNullOrEmpty(item.CountryName))
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Empty cell on Country Column Detected on line {item.ExcelLineNumber}";
                                return apiResponse;
                            }
                            else
                            {
                                var country = _country.commonLookups.FirstOrDefault(w => w.LookupName.Trim().ToLower() == item.CountryName.Trim().ToLower());
                                if(country == null)
                                {
                                    apiResponse.Status.Message.FriendlyMessage = $"Undentified Country name Detected on line {item.ExcelLineNumber}";
                                    return apiResponse;
                                }
                                item.CountryId = country.LookupId;
                            }
                            if (itemFrmRepo != null)
                            {  
                                itemFrmRepo.Address = item.Address;
                                itemFrmRepo.SupplierTypeId = item.SupplierTypeId;
                                itemFrmRepo.Name = item.Name; 
                                itemFrmRepo.Email = item.Email;
                                itemFrmRepo.PhoneNo = item.PhoneNo;
                                itemFrmRepo.RegistrationNo = item.RegistrationNo; 
                                itemFrmRepo.CountryId = itemFrmRepo.CountryId; 
                                itemFrmRepo.UpdatedOn = DateTime.Now;
                                itemFrmRepo.Website = item.Website;
                                itemFrmRepo.PostalAddress = item.PostalAddress;
                                itemFrmRepo.TaxIDorVATID = item.TaxIDorVATID;
                                itemFrmRepo.SupplierNumber = itemFrmRepo.SupplierNumber; 
                                itemFrmRepo.SupplierId = itemFrmRepo.SupplierId;
                                itemFrmRepo.HaveWorkPrintPermit = item.HaveWorkPrintPermit;
                                itemFrmRepo.Active = true;
                                itemFrmRepo.ApprovalStatusId = item.ApprovalStatusId;
                                await _repo.UpdateSupplierAsync(itemFrmRepo);
                            }
                            else
                            {
                                var newItem = new cor_supplier
                                { 
                                    Name = item.Name,
                                    Address = item.Address,
                                    PhoneNo = item.PhoneNo,
                                    Email = item.Email,
                                    RegistrationNo = item.RegistrationNo,
                                    SupplierTypeId = item.SupplierTypeId,
                                    Passport = item.Passport,
                                    CountryId = item.CountryId,
                                    ApprovalStatusId = (int)ApprovalStatus.Pending,
                                    Active = true,
                                    Deleted = false,
                                    CreatedBy = item.CreatedBy,
                                    CreatedOn = DateTime.Now,
                                    UpdatedBy = item.CreatedBy,
                                    UpdatedOn = DateTime.Now,
                                    Website = item.Website,
                                    PostalAddress = item.PostalAddress,
                                    TaxIDorVATID = item.TaxIDorVATID,
                                    SupplierNumber = item.SupplierNumber,
                                    HaveWorkPrintPermit = item.HaveWorkPrintPermit,
                                    Particulars = item.Particulars,
                                    
                                };
                                await _repo.AddNewSupplierAsync(newItem);
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
