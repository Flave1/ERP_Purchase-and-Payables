using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.Enums;
using MediatR;
using OfficeOpenXml;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Uploads_Downloads
{
    public class DownloadSupplierInformationQuery : IRequest<byte[]>
    {
        public class DownloadSupplierInformationCommand : IRequestHandler<DownloadSupplierInformationQuery, byte[]>
        {
            private readonly ISupplierRepository _repo;
            private readonly IIdentityServerRequest _serverRequest;
            public DownloadSupplierInformationCommand(
                ISupplierRepository supplierRepository,
                IIdentityServerRequest serverRequest)
            {
                _repo = supplierRepository;
                _serverRequest = serverRequest;
            }
            public async Task<byte[]> Handle(DownloadSupplierInformationQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    Byte[] File = new Byte[0];
                    var _DomainList = await _repo.GetAllSupplierAsync();
                    var _SupplierType = await _repo.GetAllSupplierTypeAsync();
                    var country = await _serverRequest.GetAllCountryAsync();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("SupplierType");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Address");
                    dt.Columns.Add("PhoneNo");
                    dt.Columns.Add("Email");
                    dt.Columns.Add("RegistrationNo"); 
                    dt.Columns.Add("SupplierNumber");
                    dt.Columns.Add("Country");
                    dt.Columns.Add("Status");
                    dt.Columns.Add("Website"); 
                    dt.Columns.Add("PostalAddress");
                    dt.Columns.Add("TaxID_VatID");
                    dt.Columns.Add("WorkPermit");
                    dt.Columns.Add("Particulars");   

                    var _ContractList = _DomainList.Select(a => new SupplierObj
                    {

                        SupplierTypeName = _SupplierType.FirstOrDefault(d => d.SupplierTypeId == a.SupplierTypeId)?.SupplierTypeName,
                        Name = a.Name,
                        Address = a.Address,
                        PhoneNo = a.PhoneNo,
                        Email = a.Email,
                        RegistrationNo = a.RegistrationNo,
                        SupplierNumber = a.SupplierNumber,
                        CountryName = country.commonLookups.FirstOrDefault(w => w.LookupId == a.CountryId)?.LookupName,
                        Website = a.Website,
                        PostalAddress = a.PostalAddress,
                        TaxIDorVATID = a.TaxIDorVATID,
                        WorkPermitName = a.HaveWorkPrintPermit ? "Yes" : "No",
                        ParticularsName = a.Particulars == 2 ? "Corporate" : "Individual" , 
                        StatusName = Convert.ToString((ApprovalStatus)a.ApprovalStatusId), 
                    }).ToList();

                    if (_ContractList.Count() > 0)
                    {
                        foreach (var itemRow in _ContractList)
                        {
                            var row = dt.NewRow();
                            row["SupplierType"] = itemRow.SupplierTypeName;
                            row["Name"] = itemRow.Name;
                            row["Address"] = itemRow.Address;
                            row["PhoneNo"] = itemRow.PhoneNo;
                            row["Email"] = itemRow.Email;
                            row["RegistrationNo"] = itemRow.RegistrationNo;
                            row["SupplierNumber"] = itemRow.SupplierNumber;
                            row["Country"] = itemRow.CountryName;
                            row["Status"] = itemRow.StatusName;
                            row["Website"] = itemRow.Website;
                            row["PostalAddress"] = itemRow.PostalAddress;
                            row["TaxID_VatID"] = itemRow.TaxIDorVATID;
                            row["WorkPermit"] = itemRow.WorkPermitName; 
                            row["Particulars"] = itemRow.ParticularsName;  
                            dt.Rows.Add(row);
                        }

                        if (_ContractList != null)
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage pck = new ExcelPackage())
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("SupplierInformation");
                                ws.DefaultColWidth = 20;
                                ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                                File = pck.GetAsByteArray();
                            }
                        }
                    }
                    return File;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
