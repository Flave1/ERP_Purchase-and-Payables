using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using OfficeOpenXml; 
using System; 
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Settup.Uploads_Downloads
{
    public class DownloadSupplierTypeQuery : IRequest<byte[]>
    {
        public class DownloadSupplierTypeQueryHandler : IRequestHandler<DownloadSupplierTypeQuery, byte[]>
        {
            private readonly ISupplierRepository _repo;
            public DownloadSupplierTypeQueryHandler
                (ISupplierRepository supplierRepository)
            {
                _repo = supplierRepository;
            }
            public async Task<byte[]> Handle(DownloadSupplierTypeQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    Byte[] File = new Byte[0];
                    var _DomainList = await _repo.GetAllSupplierTypeAsync();
                    var _TaxList = await _repo.GetAllTaxSetupAsync();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("SupplierTypeName");
                    dt.Columns.Add("TaxApplicable");
                    var _ContractList = _DomainList.Select(a => new SupplierTypeObj
                    {
                        SupplierTypeName = a.SupplierTypeName,
                        TaxApplicable = a.TaxApplicable.Split(',').Select(int.Parse).ToArray(),
                    }).ToList();

                     
                    if (_ContractList.Count() > 0)
                    {
                        foreach (var itemRow in _ContractList)
                        { 
                            var row = dt.NewRow();
                            row["SupplierTypeName"] = itemRow.SupplierTypeName;
                            row["TaxApplicable"] = string.Join(',', _TaxList.FirstOrDefault(w => itemRow.TaxApplicable.Contains(w.TaxSetupId))?.TaxName); 
                            dt.Rows.Add(row);
                        }
                        
                        if (_ContractList != null)
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage pck = new ExcelPackage())
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("SupplierType");
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
