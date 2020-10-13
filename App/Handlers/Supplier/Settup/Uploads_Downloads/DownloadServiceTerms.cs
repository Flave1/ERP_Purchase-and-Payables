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
    public class DownloadServiceTermsQuery : IRequest<byte[]>
    {
        public class DownloadServiceTermsQueryHandler : IRequestHandler<DownloadServiceTermsQuery, byte[]>
        {
            private readonly ISupplierRepository _repo;
            public DownloadServiceTermsQueryHandler
                (ISupplierRepository supplierRepository)
            {
                _repo = supplierRepository;
            }
            public async Task<byte[]> Handle(DownloadServiceTermsQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    Byte[] File = new Byte[0];
                    var _DomainList = await _repo.GetAllServiceTermsAsync();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Header");
                    dt.Columns.Add("Content"); 
                     
                    var _ContractList = _DomainList.Select(a => new ServiceTermObj
                    {
                        Header = a.Header,
                        Content = a.Content, 
                    }).ToList();
                     
                    if(_ContractList.Count() > 0)
                    {
                        foreach (var itemRow in _ContractList)
                        {
                            var row = dt.NewRow();
                            row["Header"] = itemRow.Header;
                            row["Content"] = itemRow.Content; 
                            dt.Rows.Add(row);
                        }
                        
                        if (_ContractList != null)
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage pck = new ExcelPackage())
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ServiceTerms");
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
