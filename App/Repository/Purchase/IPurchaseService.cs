using GODP.APIsContinuation.DomainObjects.Supplier;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Invoice;
using Puchase_and_payables.DomainObjects.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Repository.Purchase
{
    public interface IPurchaseService
    {

        Task SendEmailToInitiatingStaffAsync(purch_requisitionnote prn);
        Task<bool> AddUpdatePurchaseRequisitionNoteAsync(purch_requisitionnote model);
        Task<bool> AddUpdateLPOAsync(purch_plpo model);
        Task<purch_requisitionnote> GetPurchaseRequisitionNoteAsync(int Id);
        Task<IEnumerable<purch_requisitionnote>> GetAllPurchaseRequisitionNoteAsync();
        Task<bool> DeletePuchaseRequisitionNoteAsync(int Id);
        Task<purch_plpo> GetLPOsAsync(int lpoId);
        Task<purch_plpo> GetLPOByNumberAsync(string lpoNumber);
        Task<bool> AddUpdatePrnDetailsAsync(purch_prndetails model);
        Task<IEnumerable<purch_prndetails>> GetPrnDetailsByPrnId(int Id);
        Task<IEnumerable<purch_requisitionnote>> GetPRNAwaitingApprovalAsync(List<int> PrnIds, List<string> tokens);
        Task<List<purch_plpo>> GetALLLPOAsync();
        Task<bool> AddUpdateBidAndTender(cor_bid_and_tender model);
        Task<cor_bid_and_tender> GetBidAndTender(int BidAndTenderId);
        Task<IEnumerable<cor_bid_and_tender>> GetAllBidAndTender();
        Task<cor_paymentterms> GetPaymentByLPOId(int lpoId);

        Task<IEnumerable<cor_bid_and_tender>> GetAllSupplierBidAndTender(string email);
        Task<IEnumerable<cor_bid_and_tender>> GetAllPrevSupplierBidAndTender(string email);

        Task<bool> AddUpdatePaymentTermsAsync(cor_paymentterms model);
        Task<bool> DeletePaymentProposalAsync(int modelId);
        Task<List<cor_paymentterms>> GetPaymenttermsAsync();
        Task<IEnumerable<cor_paymentterms>> GetRequestedPaymentsAwaitingApprovalAsync(List<int> paymentTerms, List<string> tokens);
        Task<cor_paymentterms> GetSinglePaymenttermAsync(int Id);

        Task<IEnumerable<cor_paymentterms>> GetPaymenttermsByIdsAsync(string paymenttermsIds);

        Task<IEnumerable<cor_bid_and_tender>> GetBidAndTenderAwaitingApprovalAsync(List<int> bidAndTenderIds, List<string> tokens);

        Task<bool> CreateUpdateInvoice(purch_invoice model);
        Task<IEnumerable<purch_invoice>> GetAllInvoice();
        Task<purch_invoice> GetInvoice(int invoiceId);
        Task<IEnumerable<purch_plpo>> GetLPOAwaitingApprovalAsync(List<int> LPOIds, List<string> tokens);
        purch_plpo BuildThisBidLPO(purch_plpo thisBidLpo, cor_bid_and_tender currentBid);
        Task<bool> ShareTaxToPhasesIthereIsAsync(purch_plpo currentLpo);
        cor_bid_and_tender BuildBidAndTenderDomianObjectForNonSelectedSuppliers(purch_plpo lpo, int Requstingdpt, purch_prndetails model);
        cor_bid_and_tender BuildBidAndTenderDomianObject(cor_supplier supplier, purch_plpo lpo, int Requstingdpt, purch_prndetails model);
        purch_requisitionnote BuildPurchRequisionNoteObject(AddUpdateRequisitionNoteCommand request, int staffID, int companyId);
        List<purch_prndetails> BuildListOfPrnDetails(List<PRNDetailsObj> request, int companyId);
        purch_plpo BuildLPODomianObject(purch_prndetails model, string address, DateTime date);
         string LpoNubmer(int num);
        Task SendEmailToSuppliersAsync(EmailMessageObj em, string description);
        Task SendEmailToSuppliersSelectedAsync(int winnerSupplierid, string descsription, int lpoid);
        Task SendEmailToSuppliersWhenBidIsRejectedAsync(int Supplierid, string descsription); 
        Task RemoveLostBidsAndProposals(purch_plpo lpo);
        Task SendEmailToSupplierDetailingPaymentAsync(inv_invoice invoice, int phase);
    }
}
