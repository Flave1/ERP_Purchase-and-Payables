using GODP.APIsContinuation.Repository.Interface;
using MediatR;
using OfficeOpenXml;
using Puchase_and_payables.Contracts.GeneralExtension;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.DomainObjects.Supplier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Settup.Uploads_Downloads
{
    public class DownloadTaxSetupQuery : IRequest<byte[]>
    {
        public class DownloadTaxSetupQueryHandler : IRequestHandler<DownloadTaxSetupQuery, byte[]>
        {
            private readonly ISupplierRepository _repo;
            public DownloadTaxSetupQueryHandler
                (ISupplierRepository supplierRepository)
            {
                _repo = supplierRepository;
            }
            public async Task<byte[]> Handle(DownloadTaxSetupQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    Byte[] File = new Byte[0];
                    var _DomainList = await _repo.GetAllTaxSetupAsync();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("TaxName");
                    dt.Columns.Add("SubGL");
                    dt.Columns.Add("Percentage");
                    dt.Columns.Add("Type");
                     
                    var _ContractList = _DomainList.Select(a => new TaxsetupObj
                    {
                        TaxName = a.TaxName,
                        SubGL = a.SubGL,
                        Percentage = a.Percentage,
                        Type = a.Type
                    }).ToList();
                     
                    if(_ContractList.Count() > 0)
                    {
                        foreach (var itemRow in _ContractList)
                        {
                            var row = dt.NewRow();
                            row["TaxName"] = itemRow.TaxName;
                            row["SubGL"] = itemRow.SubGL;
                            row["Percentage"] = itemRow.Percentage;
                            row["Type"] = itemRow.Type;
                            dt.Rows.Add(row);
                        }
                        
                        if (_ContractList != null)
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage pck = new ExcelPackage())
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TaxSetup");
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
