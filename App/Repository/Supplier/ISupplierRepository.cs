using GODP.APIsContinuation.DomainObjects.Supplier;
using Puchase_and_payables.DomainObjects.Approvals;
using Puchase_and_payables.DomainObjects.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Repository.Interface
{
    public interface ISupplierRepository
    {
        Task<bool> UpdateSupplierAsync(cor_supplier model);
        Task<bool> AddNewSupplierAsync(cor_supplier model);
        Task<IEnumerable<cor_supplier>> GetAllSupplierAsync();
        Task<cor_supplier> GetSupplierAsync(int supplierId);
        Task<cor_supplier> GetSupplierByEmailAsync(string email);
        Task<bool> DeleteSupplierAsync(cor_supplier model);
        //...................
        Task<IEnumerable<cor_supplier>> GetSuppliersByCountryAsync(int countryId);

        Task<IEnumerable<cor_supplier>> SupplierSearchAsync();

        //....................
        Task<bool> UpdateSupplierAuthorizationAsync(cor_supplierauthorization model);
        Task<bool> UpdateSupplierBusinessOwnerAsync(cor_supplierbusinessowner model);
        Task<bool> UpdateSupplierDocumentAsync(cor_supplierdocument model);
        Task<bool> UpdateSupplierTopClientAsync(cor_topclient model);
        Task<bool> UpdateSupplierTopSupplierAsync(cor_topsupplier model);
        Task<bool> DeleteSupplierAuthorizationAsync(cor_supplierauthorization model);
        Task<bool> DeleteSupplierBusinessOwnerAsync(cor_supplierbusinessowner model);
        Task<bool> DeleteSupplierDocumentAsync(cor_supplierdocument model);
        Task<bool> DeleteSupplierTopClientAsync(cor_topclient model);
        Task<bool> DeleteSupplierTopSupplierAsync(cor_topsupplier model);
        Task<IEnumerable<cor_supplierauthorization>> GetAllSupplierAuthorizationAsync(int supplierId);
        Task<cor_supplierauthorization> GetSupplierAuthorizationAsync(int supplierAuthorizationId);
        Task<IEnumerable<cor_supplierbusinessowner>> GetAllSupplierBusinessOwnerAsync(int supplierId);
        Task<cor_supplierbusinessowner> GetSupplierBusinessOwnerAsync(int supplierBusinessOwnerId);
       
        Task<IEnumerable<cor_supplierdocument>> GetAllSupplierDocumentAsync(int supplierId);
        Task<cor_supplierdocument> GetSupplierDocumentAsync(int supplierDocumentId);
        Task<IEnumerable<cor_topclient>> GetAllSupplierTopClientAsync(int supplierId);
        Task<cor_topclient> GetSupplierTopClientAsync(int supplierTopClientId);
        Task<IEnumerable<cor_topsupplier>> GetAllSupplierTopSupplierAsync(int supplierId);
        Task<cor_topsupplier> GetSupplierTopSupplierAsync(int supplierTopSupplierId);
        Task<IEnumerable<cor_supplier>> SupplierInformationAwaitingApprovalAsync(string userName);
        //Task<int> GoForApprovalAsync(ApprovalReq req);
        Task<bool> UploadSupplierListAsync(byte[] record, string createdBy);
 


        //.........................
        Task<bool> AddUpdateTaxSetupAsync(cor_taxsetup model);
        Task<cor_taxsetup> GetTaxSetupAsync(int TaxSetupId);
        Task<IEnumerable<cor_taxsetup>> GetAllTaxSetupAsync();
        Task<bool> DeleteTaxSetupAsync(cor_taxsetup model);


        Task<bool> AddUpdateSeviceTermAsync(cor_serviceterms model);
        Task<cor_serviceterms> GetServiceTermsAsync(int serviceTermsId);
        Task<IEnumerable<cor_serviceterms>> GetAllServiceTermsAsync();
        Task<bool> DeleteServiceTermsAsync(cor_serviceterms model);


        Task<bool> AddUpdateSupplierTypeAsync(cor_suppliertype model);
        Task<cor_suppliertype> GetSupplierTypeAsync(int serviceTermsId);
        Task<IEnumerable<cor_suppliertype>> GetAllSupplierTypeAsync();
        Task<bool> DeleteSupplierTypeAsync(cor_suppliertype model);

        Task<IEnumerable<cor_supplier>> GetSupplierDataAwaitingApprovalAsync(List<int> SupplierIds, List<string> tokens);

    
        Task<bool> AddUpdateBankAccountdetailsAsync(cor_bankaccountdetail model);
        Task<cor_bankaccountdetail> GetBankAccountdetailAsync(int modelId);
        IEnumerable<cor_bankaccountdetail> GetAllBankAccountdetails(int supplierId);
        bool DeleteBankAccountdetail(int modelId);

        Task<bool> AddUpdateBankFinancialDetailsAsync(cor_financialdetail model);
        Task<cor_financialdetail> GetFinancialDetailAsync(int modelId);
        IEnumerable<cor_financialdetail> GetAllFinancialDetails(int supplierId);
        bool DeleteFinancialDetail(int modelId);
        Task<IEnumerable<cor_supplier>> GetListOfApprovedSuppliersAsync();

        Task<bool> AddUpdateSupplierIdentificationAsync(cor_identification model);
        bool DeleteIdentification(int modelId);
        Task<IEnumerable<cor_identification>> GetAllSupplierIdentificationAsync(int supplierId);
        Task<cor_identification> GetSupplierIdentificationAsync(int identityId);
        Task<List<cor_taxsetup>> GetTaxSetupSupplierTypeWithAsync(int supplierId);
    }
}
