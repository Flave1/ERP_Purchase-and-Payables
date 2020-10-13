using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Approvals;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Repository.Inplimentation
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        public SupplierRepository(
            DataContext dataContext, 
            IIdentityService identityService,
            IIdentityServerRequest serverRequest)
        {
            _serverRequest = serverRequest;
            _dataContext = dataContext;
        }

        public async Task<bool> AddNewSupplierAsync(cor_supplier model)
        {
            await _dataContext.cor_supplier.AddAsync(model);

            return await _dataContext.SaveChangesAsync() > 0;  
        } 
        public async Task<bool> DeleteSupplierAsync(cor_supplier model)
        {   
            var item = await _dataContext.cor_supplier.FindAsync(model.SupplierId);
            item.Deleted = true;
            _dataContext.Entry(item).CurrentValues.SetValues(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSupplierAuthorizationAsync(cor_supplierauthorization model)
        {
            var item = await _dataContext.cor_supplierauthorization.FindAsync(model.SupplierAuthorizationId);
            item.Deleted = true;
            _dataContext.Entry(item).CurrentValues.SetValues(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSupplierBusinessOwnerAsync(cor_supplierbusinessowner model)
        {
            var item = await _dataContext.cor_supplierbusinessowner.FindAsync(model.SupplierBusinessOwnerId);
            item.Deleted = true;
            _dataContext.Entry(item).CurrentValues.SetValues(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSupplierDocumentAsync(cor_supplierdocument model)
        {
            var item = await _dataContext.cor_supplierdocument.FindAsync(model.SupplierDocumentId);
            item.Deleted = true;
            _dataContext.Entry(item).CurrentValues.SetValues(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSupplierTopClientAsync(cor_topclient model)
        {
            var item = await _dataContext.cor_topclient.FindAsync(model.TopClientId);
            item.Deleted = true;
            _dataContext.Entry(item).CurrentValues.SetValues(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteSupplierTopSupplierAsync(cor_topsupplier model)
        {
            var item = await _dataContext.cor_topsupplier.FindAsync(model.TopSupplierId);
            item.Deleted = true;
            _dataContext.Entry(item).CurrentValues.SetValues(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<cor_supplier>> GetAllSupplierAsync()
        {
                var queryAble =  _dataContext.cor_supplier.AsQueryable();
            var suppliers =  await queryAble.Where(m => m.Deleted == false)
                .Include(x => x.cor_supplierauthorization)
                .Include(x => x.cor_supplierbusinessowner)
                .Include(x => x.cor_supplierdocument)
                .Include(x => x.cor_suppliertype)
                .Include(x => x.cor_topsupplier).ToListAsync();

            return suppliers.OrderByDescending(s => s.SupplierId);
        }

        public async Task<IEnumerable<cor_supplierauthorization>> GetAllSupplierAuthorizationAsync(int supplierId)
        {
            return await _dataContext.cor_supplierauthorization.Where(x => x.Deleted == false && x.SupplierId == supplierId).ToListAsync();
        }

        public async Task<IEnumerable<cor_supplierbusinessowner>> GetAllSupplierBusinessOwnerAsync(int supplierId)
        {
            return await _dataContext.cor_supplierbusinessowner.Where(x => x.Deleted == false && x.SupplierId == supplierId).ToListAsync();
        }

        public async Task<IEnumerable<cor_supplierdocument>> GetAllSupplierDocumentAsync(int supplierId)
        {
            return await _dataContext.cor_supplierdocument.Where(x => x.Deleted == false && x.SupplierId ==supplierId).ToListAsync();
        }

        public async Task<IEnumerable<cor_topclient>> GetAllSupplierTopClientAsync(int supplierId)
        {
            return await _dataContext.cor_topclient.Where(x => x.Deleted == false && x.SupplierId == supplierId).ToListAsync();
        }

        public async Task<IEnumerable<cor_topsupplier>> GetAllSupplierTopSupplierAsync(int supplierId)
        {
            return await _dataContext.cor_topsupplier.Where(x => x.Deleted == false && x.SupplierId == supplierId).ToListAsync();
        }

        public async Task<cor_supplier> GetSupplierAsync(int supplierId)
        {
            var data = await _dataContext.cor_supplier.SingleOrDefaultAsync(x => x.SupplierId == supplierId && x.Deleted == false);
            return data;
        }

        public async Task<cor_supplierauthorization> GetSupplierAuthorizationAsync(int supplierAuthorizationId)
        {
            return await _dataContext.cor_supplierauthorization.SingleOrDefaultAsync(x => x.SupplierAuthorizationId == supplierAuthorizationId
             && x.Deleted == false);
        }

        public async Task<cor_supplierbusinessowner> GetSupplierBusinessOwnerAsync(int supplierBusinessOwnerId)
        {
            return await _dataContext.cor_supplierbusinessowner.SingleOrDefaultAsync(x => x.SupplierBusinessOwnerId == supplierBusinessOwnerId
             && x.Deleted == false);
        }

        public async Task<cor_supplierdocument> GetSupplierDocumentAsync(int supplierDocumentId)
        {
            return await _dataContext.cor_supplierdocument.SingleOrDefaultAsync(x => x.SupplierDocumentId == supplierDocumentId
             && x.Deleted == false);
        }

        public async Task<IEnumerable<cor_supplier>> GetSuppliersByCountryAsync(int countryId)
        {
            return await _dataContext.cor_supplier.Where(x => x.CountryId == countryId && x.Deleted == false).ToListAsync();
        }

        public async Task<cor_topclient> GetSupplierTopClientAsync(int supplierTopClientId)
        {
            return await _dataContext.cor_topclient.SingleOrDefaultAsync(x => x.TopClientId == supplierTopClientId && x.Deleted == false);
        }

        public async Task<cor_topsupplier> GetSupplierTopSupplierAsync(int supplierTopSupplierId)
        {
            return await _dataContext.cor_topsupplier.SingleOrDefaultAsync(x => x.TopSupplierId == supplierTopSupplierId && x.Deleted == false);
        }

        public async Task<IEnumerable<cor_supplier>> SupplierInformationAwaitingApprovalAsync(string userName)
        {
            var userDetail = await _serverRequest.UserDataAsync(); 
            var thisUserSupplierData = await _dataContext.cor_supplier.Where(x => x.Email == userDetail.Email).ToListAsync();

            return thisUserSupplierData.Where(x => x.ApprovalStatusId == (int)ApprovalStatus.Pending
            && x.ApprovalStatusId == (int)ApprovalStatus.Processing);

        }

        public async  Task<IEnumerable<cor_supplier>> SupplierSearchAsync()
        {
            return await _dataContext.cor_supplier.ToListAsync();
        }

        public async Task<bool> UpdateSupplierAsync(cor_supplier model)
        {
            var supplier = await _dataContext.cor_supplier.FindAsync(model.SupplierId);
            _dataContext.Entry(supplier).CurrentValues.SetValues(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSupplierAuthorizationAsync(cor_supplierauthorization model)
        {
            if(model.SupplierAuthorizationId > 0)
            {
                var itemTUpdate = await _dataContext.cor_supplierauthorization.FindAsync(model.SupplierAuthorizationId);
                if(itemTUpdate != null)
                {
                    model.Deleted = false;
                    model.Active = true;
                    _dataContext.Entry(itemTUpdate).CurrentValues.SetValues(model);
                } 
            }else
                 await _dataContext.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSupplierBusinessOwnerAsync(cor_supplierbusinessowner model)
        {
            if (model.SupplierBusinessOwnerId > 0)
            {
                var itemTUpdate = await _dataContext.cor_supplierbusinessowner.FindAsync(model.SupplierBusinessOwnerId);
                if(itemTUpdate != null)
                {
                    model.Deleted = false;
                    model.Active = true;
                    _dataContext.Entry(itemTUpdate).CurrentValues.SetValues(model);
                } 
            }
            else
                await _dataContext.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSupplierDocumentAsync(cor_supplierdocument model)
        {
            if (model.SupplierDocumentId > 0)
            {
                var itemTUpdate = await _dataContext.cor_supplierdocument.FindAsync(model.SupplierDocumentId);
                if(itemTUpdate != null)
                { 
                    _dataContext.Entry(itemTUpdate).CurrentValues.SetValues(model);
                }
            }else
                await _dataContext.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSupplierTopClientAsync(cor_topclient model)
        {
            if (model.TopClientId > 0)
            {
                var itemTUpdate = await _dataContext.cor_topclient.FindAsync(model.TopClientId);
                if(itemTUpdate != null)
                { 
                    _dataContext.Entry(itemTUpdate).CurrentValues.SetValues(model);
                }
            }else
                await _dataContext.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSupplierTopSupplierAsync(cor_topsupplier model)
        {
            if (model.TopSupplierId > 0)
            {
                var itemTUpdate = await _dataContext.cor_topsupplier.FindAsync(model.TopSupplierId);
                if(itemTUpdate != null)
                {
                    _dataContext.Entry(itemTUpdate).CurrentValues.SetValues(model); 
                }
            }
            else
                await _dataContext.cor_topsupplier.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }
         
        public async Task<bool> UploadSupplierListAsync(byte[] record, string createdBy)
        {
            try
            {
                var updated = 0;
                var uploadedRecord = new List<cor_supplier>();
                using (MemoryStream stream = new MemoryStream(record))

                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    //Use first sheet by default
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];
                    int totalRows = workSheet.Dimension.Rows;
                    //First row is considered as the header
                    for (int i = 2; i <= totalRows; i++)
                    {
                        uploadedRecord.Add(new cor_supplier
                        {
                            Name = workSheet.Cells[i, 1].Value.ToString(),
                            //SupplierTypeId = SupplierTypeBySupplierTypeName(workSheet.Cells[i, 2].Value.ToString()),
                            Address = workSheet.Cells[i, 3].Value.ToString(),
                            Email = workSheet.Cells[i, 2].Value.ToString(),
                            PostalAddress = workSheet.Cells[i, 3].Value.ToString(),
                            //CountryId = CountryIdByCountryName(workSheet.Cells[i, 2].Value.ToString()),
                            PhoneNo = workSheet.Cells[i, 1].Value.ToString(),
                            RegistrationNo = workSheet.Cells[i, 1].Value.ToString(),
                            Active = true,
                            Deleted = false,
                            CreatedBy = createdBy,
                            CreatedOn = DateTime.Now,
                            UpdatedBy = createdBy,
                            UpdatedOn = DateTime.Now,
                        });
                    }

                }
                if (uploadedRecord.Count > 0)
                {
                    foreach (var item in uploadedRecord)
                    {
                        var supplierFrmDB = await _dataContext.cor_supplier.FirstOrDefaultAsync(x => (
                        x.Name.Trim().ToLower() == item.Name.Trim().ToLower() ||
                        x.SupplierNumber.Trim().ToLower() == item.Name.Trim().ToLower() ||
                        x.Email.ToLower().Trim() == item.Email.ToLower().Trim()) && x.Deleted == false);

                        if (supplierFrmDB != null)
                        {
                            supplierFrmDB.SupplierNumber = item.SupplierNumber;
                            supplierFrmDB.Name = item.Name;
                            supplierFrmDB.TaxIDorVATID = item.TaxIDorVATID;
                            supplierFrmDB.RegistrationNo = item.RegistrationNo;
                            supplierFrmDB.PostalAddress = item.PostalAddress;
                            supplierFrmDB.Address = item.Address;
                            supplierFrmDB.PhoneNo = item.PhoneNo;
                            supplierFrmDB.Email = item.Email;
                            supplierFrmDB.CountryId = item.CountryId;
                            supplierFrmDB.Active = true;
                            supplierFrmDB.Deleted = false;
                            supplierFrmDB.CreatedBy = createdBy;
                            supplierFrmDB.CreatedOn = DateTime.Now;
                            supplierFrmDB.UpdatedBy = createdBy;
                            supplierFrmDB.UpdatedOn = DateTime.Now;
                            _dataContext.Entry(supplierFrmDB).State = EntityState.Modified;
                        }
                        else
                        {
                            var newSupplier = new cor_supplier
                            {
                                SupplierNumber = item.SupplierNumber,
                                Name = item.Name,
                                TaxIDorVATID = item.TaxIDorVATID,
                                RegistrationNo = item.RegistrationNo,
                                PostalAddress = item.PostalAddress,
                                Address = item.Address,
                                PhoneNo = item.PhoneNo,
                                Email = item.Email,
                                CountryId = item.CountryId,
                                Active = true,
                                Deleted = false,
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.Now,
                                UpdatedBy = createdBy,
                                UpdatedOn = DateTime.Now,
                            };
                            _dataContext.cor_supplier.Add(newSupplier);
                        }
                    }

                    updated = await _dataContext.SaveChangesAsync();

                }
                return updated > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<bool> AddUpdateSeviceTermAsync(cor_serviceterms model)
        {
            if (model.ServiceTermsId > 0)
            {
                var itemToEdit = await _dataContext.cor_serviceterms.FindAsync(model.ServiceTermsId);
                _dataContext.Entry(itemToEdit).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.cor_serviceterms.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddUpdateSupplierTypeAsync(cor_suppliertype model)
        {
            if (model.SupplierTypeId > 0)
            {
                var itemToEdit = await _dataContext.cor_suppliertype.FindAsync(model.SupplierTypeId);
                _dataContext.Entry(itemToEdit).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.cor_suppliertype.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddUpdateTaxSetupAsync(cor_taxsetup model)
        {
            if (model.TaxSetupId > 0)
            {
                var itemToEdit = await _dataContext.cor_taxsetup.FindAsync(model.TaxSetupId);
                _dataContext.Entry(itemToEdit).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.cor_taxsetup.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteServiceTermsAsync(cor_serviceterms model)
        {
            var itemToDelete = await _dataContext.cor_serviceterms.FindAsync(model.ServiceTermsId);  
            if(itemToDelete != null)
            {
                _dataContext.cor_serviceterms.Remove(itemToDelete);
            } 
            return await _dataContext.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteSupplierTypeAsync(cor_suppliertype model)
        {
            var itemToDelete = await _dataContext.cor_suppliertype.FindAsync(model.SupplierTypeId);
            if(itemToDelete != null)
            {
                _dataContext.cor_suppliertype.Remove(itemToDelete); 
            }
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTaxSetupAsync(cor_taxsetup model)
        {
            var itemToDelete = await _dataContext.cor_taxsetup.FindAsync(model.TaxSetupId); 
            if(itemToDelete != null)
            {
                _dataContext.cor_taxsetup.Remove(itemToDelete);
            }
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<cor_taxsetup>> GetAllTaxSetupAsync()
        {
            return await _dataContext.cor_taxsetup.Where(s => s.Deleted == false).ToListAsync();
        }

        public async Task<cor_serviceterms> GetServiceTermsAsync(int serviceTermsId)
        {
            return await _dataContext.cor_serviceterms.FirstOrDefaultAsync(s => s.Deleted == false && s.ServiceTermsId ==serviceTermsId);
        }

        public async Task<IEnumerable<cor_serviceterms>> GetAllServiceTermsAsync()
        {
            return await _dataContext.cor_serviceterms.Where(s => s.Deleted == false).ToListAsync();
        }

        public async Task<cor_suppliertype> GetSupplierTypeAsync(int supplierTypeId)
        {
            return await _dataContext.cor_suppliertype.FirstOrDefaultAsync(s => s.SupplierTypeId == supplierTypeId);
        }

        public async Task<IEnumerable<cor_suppliertype>> GetAllSupplierTypeAsync()
        {
            return await _dataContext.cor_suppliertype.Where(s => s.Deleted == false).ToListAsync();
        }

        public async Task<cor_taxsetup> GetTaxSetupAsync(int TaxSetupId)
        {
            return await _dataContext.cor_taxsetup.FirstOrDefaultAsync(s => s.TaxSetupId == TaxSetupId);
        }

        public async Task<IEnumerable<cor_supplier>> GetSupplierDataAwaitingApprovalAsync(List<int> SupplierIds, List<string> tokens)
        {
            var item = await _dataContext.cor_supplier
                .Where(s => SupplierIds.Contains(s.SupplierId)
                && s.Deleted == false && tokens.Contains(s.WorkflowToken)).ToListAsync();
            return item;
        }

     

        public async Task<bool> AddUpdateBankAccountdetailsAsync(cor_bankaccountdetail model)
        {
            if (model.BankAccountDetailId > 0)
            {
                var itemToEdit = await _dataContext.cor_bankaccountdetail.FindAsync(model.BankAccountDetailId);
                _dataContext.Entry(itemToEdit).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.cor_bankaccountdetail.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<cor_bankaccountdetail> GetBankAccountdetailAsync(int modelId)
        {
            return await _dataContext.cor_bankaccountdetail.FirstOrDefaultAsync(s => s.Deleted == false && s.BankAccountDetailId == modelId);
        }

        public IEnumerable<cor_bankaccountdetail> GetAllBankAccountdetails(int supplierId)
        {
            return  _dataContext.cor_bankaccountdetail.Where(s => s.Deleted == false && s.SupplierId == supplierId).ToList();
        }

        public bool DeleteBankAccountdetail(int modelId)
        {
            var itemToDelete = _dataContext.cor_bankaccountdetail.Find(modelId);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return  _dataContext.SaveChanges() > 0;
        }

        public async Task<bool> AddUpdateBankFinancialDetailsAsync(cor_financialdetail model)
        {
            if (model.FinancialdetailId > 0)
            {
                var itemToEdit = await _dataContext.cor_bankaccountdetail.FindAsync(model.FinancialdetailId);
                _dataContext.Entry(itemToEdit).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.cor_financialdetail.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<cor_financialdetail> GetFinancialDetailAsync(int modelId)
        {
            return await _dataContext.cor_financialdetail.FirstOrDefaultAsync(s => s.Deleted == false && s.FinancialdetailId == modelId);
        }

        public IEnumerable<cor_financialdetail> GetAllFinancialDetails(int supplierId)
        {
            return _dataContext.cor_financialdetail.Where(s => s.Deleted == false && s.SupplierId ==supplierId).ToList();
        }

        public bool DeleteFinancialDetail(int modelId)
        {
            var itemToDelete = _dataContext.cor_financialdetail.Find(modelId);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return  _dataContext.SaveChanges() > 0;
        }

        public async Task<cor_supplier> GetSupplierByEmailAsync(string email)
        {
            var data =  await _dataContext.cor_supplier.FirstOrDefaultAsync(d => d.Deleted == false && d.Email.Trim().ToLower() == email.Trim().ToLower());
            return data;
        }

        public async Task<IEnumerable<cor_supplier>> GetListOfApprovedSuppliersAsync()
        {
            return await _dataContext.cor_supplier.Where(d => d.ApprovalStatusId == (int)ApprovalStatus.Approved && d.Deleted == false).ToListAsync();
        }

        public async Task<bool> AddUpdateSupplierIdentificationAsync(cor_identification model)
        {
            if (model.IdentificationId > 0)
            {
                var itemToEdit = await _dataContext.cor_identification.FindAsync(model.IdentificationId);
                _dataContext.Entry(itemToEdit).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.cor_identification.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<cor_identification>> GetAllSupplierIdentificationAsync(int supplierId)
        {
            return await _dataContext.cor_identification.Where(d => d.SupplierId == supplierId && d.Deleted == false).ToListAsync();
        }

        public async Task<cor_identification> GetSupplierIdentificationAsync(int identityId)
        {
            return await _dataContext.cor_identification.SingleOrDefaultAsync(d => d.IdentificationId == identityId && d.Deleted == false);
        }

        public bool DeleteIdentification(int modelId)
        {
            var itemToDelete = _dataContext.cor_identification.Find(modelId);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return _dataContext.SaveChanges() > 0;
        }

        public async Task<List<cor_taxsetup>> GetTaxSetupSupplierTypeWithAsync(int supplierId)
        {
            var supplier = await GetSupplierAsync(supplierId);
            if (supplier != null)
            {
                var supplierType = await GetSupplierTypeAsync(supplier.SupplierTypeId);
                if (supplierType != null)
                {
                    var TaxIds = supplierType.TaxApplicable.Split(',').Select(int.Parse).Distinct();
                    var ListOfTaxapplicable = new List<cor_taxsetup>();
                    foreach (var tId in TaxIds)
                    {
                        var taxApplicable = await GetTaxSetupAsync(tId);
                        if (taxApplicable != null)
                        {
                            ListOfTaxapplicable.Add(taxApplicable);
                        }
                    }
                    return ListOfTaxapplicable;
                }
            }
            return new List<cor_taxsetup>();
        }
    }
}
