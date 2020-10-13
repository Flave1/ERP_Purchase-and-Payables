using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.URI;
using Microsoft.EntityFrameworkCore;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Queries.Finanace;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Invoice;
using Puchase_and_payables.DomainObjects.Purchase;  
using Puchase_and_payables.Helper.Extensions;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Repository.Purchase
{
    public class PurchaseService : IPurchaseService
    {
        private readonly DataContext _dataContext;
        private readonly ISupplierRepository _supRepo;
        private readonly IBaseURIs _uRIs; 
        private readonly IIdentityServerRequest _serverRequest;
        public PurchaseService(
            DataContext dataContext,
            ISupplierRepository supplierRepository,
            IBaseURIs uRIs, 
            IIdentityServerRequest serverRequest)
        {
            _serverRequest = serverRequest;
            _dataContext = dataContext;
            _uRIs = uRIs;
            _supRepo = supplierRepository;
        }
         
        public string LpoNubmer(int num)
        {
            return $"{LPONumber.Generate(num)}";
        }
        public async Task<bool> AddUpdatePrnDetailsAsync(purch_prndetails model)
        {
            if (model.PRNDetailsId > 0)
            {
                var itemTUpdate = await _dataContext.purch_prndetails.FindAsync(model.PRNDetailsId);
                _dataContext.Entry(itemTUpdate).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.purch_prndetails.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddUpdatePurchaseRequisitionNoteAsync(purch_requisitionnote model)
        {
            if (model.PurchaseReqNoteId > 0)
            {
                var itemTUpdate = await _dataContext.purch_requisitionnote.FindAsync(model.PurchaseReqNoteId);
                _dataContext.Entry(itemTUpdate).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.purch_requisitionnote.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddUpdateLPOAsync(purch_plpo model)
        {
            if (model.PLPOId > 0)
            {
                var itemTUpdate = await _dataContext.purch_plpo.FindAsync(model.PLPOId);
                _dataContext.Entry(itemTUpdate).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.purch_plpo.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePuchaseRequisitionNoteAsync(int Id)
        {
            var item = await _dataContext.purch_requisitionnote.FindAsync(Id);
            item.Deleted = true;
            _dataContext.Entry(item).CurrentValues.SetValues(item);
            var prnDetails = _dataContext.purch_prndetails.Where(d => d.PurchaseReqNoteId == Id).ToList();
            foreach (var prn in prnDetails)
            {
                prn.Deleted = true;
                _dataContext.Entry(prn).CurrentValues.SetValues(prn);
            }
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<purch_requisitionnote>> GetAllPurchaseRequisitionNoteAsync()
        {
            return await _dataContext.purch_requisitionnote.Where(d => d.Deleted == false).ToListAsync();

        }

        public async Task<IEnumerable<purch_prndetails>> GetPrnDetailsByPrnId(int Id)
        {
            return await _dataContext.purch_prndetails.Where(d => d.PurchaseReqNoteId == Id).ToListAsync();
        }

        public async Task<purch_plpo> GetLPOsAsync(int lpoId)
        {
            return await _dataContext.purch_plpo.FindAsync(lpoId);
        }

        public async Task<purch_requisitionnote> GetPurchaseRequisitionNoteAsync(int Id)
        {
            return await _dataContext.purch_requisitionnote.FindAsync(Id);
        }

        public async Task<IEnumerable<purch_requisitionnote>> GetPRNAwaitingApprovalAsync(List<int> prnIds, List<string> tokens)
        {
            var item = await _dataContext.purch_requisitionnote
                .Where(s => prnIds.Contains(s.PurchaseReqNoteId)
                && s.Deleted == false && tokens.Contains(s.WorkflowToken)).ToListAsync();
            return item;
        }

        public async Task<IEnumerable<cor_bid_and_tender>> GetBidAndTenderAwaitingApprovalAsync(List<int> bidAndTenderIds, List<string> tokens)
        {
            var item = await _dataContext.cor_bid_and_tender
                .Where(s => bidAndTenderIds.Contains(s.BidAndTenderId)
                && tokens.Contains(s.WorkflowToken)).ToListAsync();
            return item;
        }

        public async Task<IEnumerable<purch_plpo>> GetLPOAwaitingApprovalAsync(List<int> LPOIds, List<string> tokens)
        {
            var item = await _dataContext.purch_plpo
                .Where(s => LPOIds.Contains(s.PLPOId)
                && tokens.Contains(s.WorkflowToken)).ToListAsync();
            return item;
        }


        public async Task<IEnumerable<cor_paymentterms>> GetRequestedPaymentsAwaitingApprovalAsync(List<int> paymentTerms, List<string> tokens)
        {
            var item = await _dataContext.cor_paymentterms
                .Where(s => paymentTerms.Contains(s.PaymentTermId)
                && tokens.Contains(s.WorkflowToken)).ToListAsync();
            return item;
        }
        public async Task<List<purch_plpo>> GetALLLPOAsync()
        {
            var item = await _dataContext.purch_plpo.Where(s => s.Deleted == false).ToListAsync();
            return item;
        }

        public async Task<bool> AddUpdateBidAndTender(cor_bid_and_tender model)
        {
            try
            {
                if (model.BidAndTenderId > 0)
                {
                    var itemToUpdate = await _dataContext.cor_bid_and_tender.FindAsync(model.BidAndTenderId);
                    _dataContext.Entry(itemToUpdate).CurrentValues.SetValues(model);
                }
                else
                {
                    await _dataContext.cor_bid_and_tender.AddAsync(model);
                }
                if (model.Paymentterms != null)
                {
                    if (model.Paymentterms.Count() > 0)
                    {
                        foreach (var item in model.Paymentterms.ToList())
                        {
                            await AddUpdatePaymentTermsAsync(item);
                        }
                    }
                }
                return await _dataContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            } 
        } 
        public async Task<cor_bid_and_tender> GetBidAndTender(int BidAndTenderId)
        {
            var bid = await _dataContext.cor_bid_and_tender.FirstOrDefaultAsync(d => d.BidAndTenderId == BidAndTenderId) ?? new cor_bid_and_tender();
            bid.Paymentterms = _dataContext.cor_paymentterms.Where(q => q.BidAndTenderId == bid.BidAndTenderId).ToList();
            return bid;
        }

        public async Task<IEnumerable<cor_bid_and_tender>> GetAllBidAndTender()
        {
            return await _dataContext.cor_bid_and_tender.ToListAsync();
        }

        public async Task<IEnumerable<cor_bid_and_tender>> GetAllSupplierBidAndTender(string email)
        {
            var supplier = await _supRepo.GetSupplierByEmailAsync(email);
            if (supplier == null) return new List<cor_bid_and_tender>();
            return await _dataContext.cor_bid_and_tender.Where(d => d.SupplierId == supplier.SupplierId && d.ApprovalStatusId == (int)ApprovalStatus.Awaiting).ToListAsync();
        }
        public async Task<IEnumerable<cor_bid_and_tender>> GetAllPrevSupplierBidAndTender(string email)
        {
            var supplier = await _supRepo.GetSupplierByEmailAsync(email);
            if (supplier == null) return new List<cor_bid_and_tender>();
            return await _dataContext.cor_bid_and_tender.Where(d => d.SupplierId == supplier.SupplierId).ToListAsync();
        }



        public async Task<bool> AddUpdatePaymentTermsAsync(cor_paymentterms model)
        {
            if (model.PaymentTermId > 0)
            {
                var itemToUpdate = await _dataContext.cor_paymentterms.FindAsync(model.PaymentTermId);
                _dataContext.Entry(itemToUpdate).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.cor_paymentterms.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<List<cor_paymentterms>> GetPaymenttermsAsync()
        {
            return await _dataContext.cor_paymentterms.ToListAsync();
        }

        public async Task<purch_plpo> GetLPOByNumberAsync(string lpoNumber)
        {
            return await _dataContext.purch_plpo.FirstOrDefaultAsync(d => d.Deleted == false && d.LPONumber.ToLower().Trim() == lpoNumber.ToLower().Trim());
        }

        public async Task<bool> CreateUpdateInvoice(purch_invoice model)
        {
            if (model.InvoiceId > 0)
            {
                var itemToUpdate = await _dataContext.purch_invoice.FindAsync(model.InvoiceId);
                _dataContext.Entry(itemToUpdate).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.purch_invoice.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<purch_invoice>> GetAllInvoice()
        {
            return await _dataContext.purch_invoice.ToListAsync();
        }

        public async Task<purch_invoice> GetInvoice(int invoiceId)
        {
            return await _dataContext.purch_invoice.FirstOrDefaultAsync(d => d.InvoiceId == invoiceId);
        }

        public async Task<IEnumerable<cor_paymentterms>> GetPaymenttermsByIdsAsync(string paymenttermsIds)
        {
            var Ids = new List<int>();
            Ids = paymenttermsIds.Split(',').Select(int.Parse).ToList();
            return await _dataContext.cor_paymentterms.Where(d => Ids.Contains(d.PaymentTermId)).ToListAsync();
        }

        public async Task<cor_paymentterms> GetSinglePaymenttermAsync(int Id)
        {
            return await _dataContext.cor_paymentterms.FirstOrDefaultAsync(d => d.PaymentTermId == Id) ?? new cor_paymentterms();
        }

        public async Task<cor_paymentterms> GetPaymentByLPOId(int lpoId)
        {
            return await _dataContext.cor_paymentterms.LastOrDefaultAsync(q => q.LPOId == lpoId) ?? new cor_paymentterms();
        }

        public async Task<bool> DeletePaymentProposalAsync(int modelId)
        {
            var item = await _dataContext.cor_paymentterms.FindAsync(modelId);
            if (item != null)
            {
                _dataContext.cor_paymentterms.Remove(item);
            }
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public cor_bid_and_tender BuildBidAndTenderDomianObject(cor_supplier supplier, purch_plpo lpo, int Requstingdpt, purch_prndetails model)
        {
            return new cor_bid_and_tender
            {
                PurchaseReqNoteId = model.PurchaseReqNoteId,
                DescriptionOfRequest = lpo.Description,
                Location = lpo.Address,
                LPOnumber = lpo.LPONumber,
                ProposedAmount = 0,
                RequestDate = DateTime.Today,
                RequestingDepartment = Requstingdpt,
                SupplierName = supplier.Name,
                Suppliernumber = supplier.SupplierNumber,
                AmountApproved = lpo.Total,
                DecisionResult = (int)DecisionResult.Non_Applicable,
                PLPOId = lpo.PLPOId,
                SupplierId = supplier.SupplierId,
                Quantity = model.Quantity,
                Total = model.SubTotal,
                ApprovalStatusId = (int)ApprovalStatus.Awaiting,
                SupplierAddress = supplier.Address,
                ExpectedDeliveryDate = lpo.DeliveryDate,
                CompanyId = model.CompanyId,
                Comment = model.Comment
            };
        }
        public cor_bid_and_tender BuildBidAndTenderDomianObjectForNonSelectedSuppliers(purch_plpo lpo, int Requstingdpt, purch_prndetails model)
        {
            return new cor_bid_and_tender
            {
                PurchaseReqNoteId = model.PurchaseReqNoteId,
                DescriptionOfRequest = lpo.Description,
                Location = lpo.Address,
                LPOnumber = lpo.LPONumber,
                ProposedAmount = 0,
                RequestDate = DateTime.Today,
                RequestingDepartment = Requstingdpt,
                SupplierName = string.Empty,
                Suppliernumber = string.Empty,
                AmountApproved = lpo.Total,
                DecisionResult = (int)DecisionResult.Non_Applicable,
                PLPOId = lpo.PLPOId,
                SupplierId = 0,
                Quantity = model.Quantity,
                Total = model.SubTotal,
                ApprovalStatusId = (int)ApprovalStatus.Awaiting,
                SupplierAddress = string.Empty,
                ExpectedDeliveryDate = lpo.DeliveryDate,
                CompanyId = model.CompanyId,
                Comment = model.Comment,
                SelectedSuppliers = string.Join(',', lpo.SupplierIds),
            };
        }

        public purch_plpo BuildThisBidLPO(purch_plpo thisBidLpo, cor_bid_and_tender currentBid)
        {
            try
            {
                var TaxSetup = _supRepo.GetTaxSetupSupplierTypeWithAsync(currentBid.SupplierId).Result;
                var supplier = _supRepo.GetSupplierAsync(currentBid.SupplierId).Result;
                var amount = _dataContext.cor_paymentterms.Where(e => e.BidAndTenderId == currentBid.BidAndTenderId && e.ProposedBy == (int)Proposer.STAFF).Sum(q => q.Amount);
                if (supplier != null)
                {
                    var supplierType = _supRepo.GetSupplierTypeAsync(supplier.SupplierTypeId).Result;
                    thisBidLpo.JobStatus = (int)JobProgressStatus.Not_Started;
                    thisBidLpo.BidComplete = true;
                    thisBidLpo.Name = supplier.Name;
                    thisBidLpo.SupplierAddress = currentBid.Location;
                    thisBidLpo.GrossAmount = currentBid.Total;
                    thisBidLpo.BidAndTenderId = currentBid.BidAndTenderId;
                    thisBidLpo.Description = currentBid.DescriptionOfRequest;
                    thisBidLpo.WinnerSupplierId = currentBid.SupplierId;
                    thisBidLpo.Total = currentBid.Total;
                    thisBidLpo.ApprovalStatusId = (int)ApprovalStatus.Pending;
                    thisBidLpo.Location = currentBid.Location;
                    thisBidLpo.Quantity = currentBid.Quantity;
                    thisBidLpo.AmountPayable = amount;
                    thisBidLpo.CompanyId = currentBid.CompanyId;
                    thisBidLpo.SupplierAddress = supplier.Address;
                    thisBidLpo.SupplierNumber = supplier.SupplierNumber;
                    thisBidLpo.Description = currentBid.DescriptionOfRequest;
                }

                if (TaxSetup.Count() > 0)
                {
                    decimal charge = new decimal();
                    decimal deduction = new decimal();
                    thisBidLpo.Taxes = string.Join(',', TaxSetup.Select(s => s.TaxSetupId));
                    foreach (var tax in TaxSetup)
                    {
                        if (tax.Type == "+")
                        {
                            // (80 / 100) * 25
                            charge = thisBidLpo.GrossAmount / 100 * Convert.ToDecimal(tax.Percentage) + charge;
                        }
                        if (tax.Type == "-")
                        {
                            deduction = thisBidLpo.GrossAmount / 100 * Convert.ToDecimal(tax.Percentage) + deduction;
                        }
                    }
                    thisBidLpo.AmountPayable = thisBidLpo.GrossAmount - deduction + charge;
                    thisBidLpo.Tax = (thisBidLpo.AmountPayable - thisBidLpo.GrossAmount);

                }
                return thisBidLpo;
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        public purch_requisitionnote BuildPurchRequisionNoteObject(AddUpdateRequisitionNoteCommand request, int staffID, int companyId)
        {
            return new purch_requisitionnote
            {
                Total = request.Total,
                PurchaseReqNoteId = request.PurchaseReqNoteId,
                Active = true,
                ApprovalStatusId = (int)ApprovalStatus.Processing,
                Comment = request.Comment,
                CreatedBy = request.RequestBy,
                RequestBy = request.RequestBy,
                Deleted = false,
                CompanyId = companyId,
                CreatedOn = DateTime.Now,
                DeliveryLocation = request.DeliveryLocation,
                DepartmentId = request.DepartmentId,
                Description = request.Description,
                DocumentNumber = request.DocumentNumber,
                IsFundAvailable = request.IsFundAvailable,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                UpdatedBy = request.RequestBy,
                UpdatedOn = DateTime.Today,
                PRNNumber = $"{DateTime.Now.Month}{DateTime.Now.Day}{PRNNumber.Generate(10)}",
                StaffId = staffID
            };
        }
        public List<purch_prndetails> BuildListOfPrnDetails(List<PRNDetailsObj> request, int companyId)
        {
            List<purch_prndetails> dts = new List<purch_prndetails>();
            foreach (var requestItem in request)
            {
                var dt = new purch_prndetails
                {
                    UpdatedOn = DateTime.Today,
                    Active = true,
                    CreatedOn = DateTime.Now,
                    Deleted = true,
                    Description = requestItem.Description,
                    IsBudgeted = requestItem.IsBudgeted,
                    PRNDetailsId = requestItem.PRNDetailsId,
                    Quantity = requestItem.Quantity,
                    SubTotal = requestItem.SubTotal,
                    SuggestedSupplierId = string.Join(',', requestItem.SuggestedSupplierId),
                    UnitPrice = requestItem.UnitPrice,
                    CompanyId = companyId,
                    Comment = requestItem.Comment
                };
                dts.Add(dt);
            }
            return dts;
        }
        public async Task<bool> ShareTaxToPhasesIthereIsAsync(purch_plpo currentLpo)
        {
           
            var paymentPhases = _dataContext.cor_paymentterms.Where(q => q.LPOId == currentLpo.PLPOId && q.ProposedBy == (int)Proposer.STAFF).ToList();

            var selected = currentLpo.Taxes.Split(',').Select(int.Parse).ToList();
            var taxes = _dataContext.cor_taxsetup.Where(e => selected.Contains(e.TaxSetupId)).ToList();

            if (paymentPhases.Count() > 0)
            {
                foreach (var phase in paymentPhases)
                {
                    if (phase.Amount != 0)
                    {

                        decimal charge = new decimal();
                        decimal deduction = new decimal(); 
                        foreach (var tax in taxes)
                        {
                            if (tax.Type == "+")
                            {
                                charge = phase.Amount / 100 * Convert.ToDecimal(tax.Percentage) + charge;
                            }
                            if (tax.Type == "-")
                            {
                                deduction = phase.Amount / 100 * Convert.ToDecimal(tax.Percentage) + deduction;
                            }
                        }
                        phase.NetAmount = phase.Amount - deduction + charge;
                        phase.TaxPercent = Convert.ToDouble(phase.NetAmount - phase.Amount);  
                    }
                    await AddUpdatePaymentTermsAsync(phase);
                }
            }
            return await Task.Run(() => true);
        }
        public purch_plpo BuildLPODomianObject(purch_prndetails model, string address, DateTime date)
        {
            return new purch_plpo
            {
                Active = true,
                DeliveryDate = date,
                Address = address,
                ApprovalStatusId = (int)ApprovalStatus.Pending,
                Description = model.Description,
                LPONumber = model.LPONumber,
                Name = model.LPONumber,
                Deleted = false,
                SupplierIds = model.SuggestedSupplierId,
                Total = model.SubTotal,
                RequestDate = DateTime.Today,
                PurchaseReqNoteId = model.PurchaseReqNoteId,
            };
        }
     

        public async Task RemoveLostBidsAndProposals(purch_plpo lpo)
        {
            var lostbids = _dataContext.cor_bid_and_tender.Where(a => a.PLPOId == lpo.PLPOId &&  a.BidAndTenderId != lpo.WinnerSupplierId).ToList();
            if(lostbids.Count() > 0)
            {
                var lostProposals = _dataContext.cor_paymentterms.Where(b => lostbids.Select(q => q.BidAndTenderId).Contains(b.BidAndTenderId)).ToList();
                if(lostProposals.Count() > 0)
                {
                    _dataContext.cor_paymentterms.RemoveRange(lostProposals);
                }
                _dataContext.cor_bid_and_tender.RemoveRange(lostbids);
            }
        }
        public async Task SendEmailToSuppliersAsync(EmailMessageObj em, string description)
        {

            var path = $"#/main/adverts";

            var path2 = $"{_uRIs.SelfClient}/{path}";
            em.Subject = "Selected to supply";
            em.Content = $"Dear Supplier <br> You have been selected to bid for  {description} from your advert page <br>" +
                $"Kindly provide a proposal breakdown for the supply of {description} <br>" +
                $"You can also upload your proposal terms" +
                $"<br>Please click <a href='{path2}'> here </a> to see adverts";
            em.FromAddresses = new List<EmailAddressObj>();
            em.SendIt = true;
            em.SaveIt = false;
            em.Template = (int)EmailTemplate.Advert;

            await _serverRequest.SendMessageAsync(em);
        }

        public async Task SendEmailToSupplierDetailingPaymentAsync(inv_invoice invoice, int phase)
        {

            EmailMessageObj em = new EmailMessageObj { ToAddresses = new List<EmailAddressObj>() };
            var supplier = await _dataContext.cor_supplier.FindAsync(invoice.SupplierId);
            if (supplier != null)
            {
                em.ToAddresses.Add(new EmailAddressObj { Address = supplier.Email, Name = supplier.Name });
            }

            var path = $"#/main/lpos";

            var path2 = $"{_uRIs.SelfClient}/{path}";
            em.Subject = $"Phase {phase} Payment";
            em.Content = $"Dear Supplier <br> Invoice Number '{invoice.InvoiceNumber}' Phase '{phase}'" +
                $" has successfully been paid for." +
                $"<br>Please click <a href='{path2}'> here </a> to see details of Payment";
            em.FromAddresses = new List<EmailAddressObj>();
            em.SendIt = true;
            em.SaveIt = false;
            em.Template = (int)EmailTemplate.Default;

            await _serverRequest.SendMessageAsync(em);
        }
        public async Task SendEmailToSuppliersSelectedAsync(int winnerSupplierid, string descsription, int id)
        {

            EmailMessageObj em = new EmailMessageObj { ToAddresses = new List<EmailAddressObj>()};
            var supplier = await _dataContext.cor_supplier.FindAsync(winnerSupplierid);
            if (supplier != null)
            {
                em.ToAddresses.Add(new EmailAddressObj { Address = supplier.Email, Name = supplier.Name });
            } 

            var path = $"#/main/lpos";

            var path2 = $"{_uRIs.SelfClient}/{path}";
            em.Subject = "Bid Selected";
            em.Content = $"Dear {supplier.Name} <br> Congratulations you have been successfully selected for the supplier of '{descsription}'" + 
                $"<br><b>Note :</b> Before the final approval of the Local purchase order (LPO), you can be able to reject the supply of {descsription}. <br> Please click  <a href='{path2}'> here</a> to see the approved proposal breakdown for the supplier of {descsription}";
            em.FromAddresses = new List<EmailAddressObj>();
            em.SendIt = true;
            em.SaveIt = false;
            em.Template = (int)EmailTemplate.Advert;

            await _serverRequest.SendMessageAsync(em);
        }

        public async Task SendEmailToSuppliersWhenBidIsRejectedAsync(int Supplierid, string descsription)
            {

            EmailMessageObj em = new EmailMessageObj { ToAddresses = new List<EmailAddressObj>() };
            var supplier = await _dataContext.cor_supplier.FindAsync(Supplierid);
            if (supplier != null)
            {
                em.ToAddresses.Add(new EmailAddressObj { Address = supplier.Email, Name = supplier.Name });
                var path = $"#/main/adverts";

                var path2 = $"{_uRIs.SelfClient}/{path}";
                em.Subject = "LPO Response";
                em.Content = $"Dear Supplier <br> You have successfully rejected the supply for this item  '{descsription}'" +
                    $"<br>Please click <a href='{path2}'> here </a> to see other available averts";
                em.FromAddresses = new List<EmailAddressObj>();
                em.SendIt = true;
                em.SaveIt = false;
                em.Template = (int)EmailTemplate.Default;

                await _serverRequest.SendMessageAsync(em);
            }

            
        }

        public async Task SendEmailToInitiatingStaffAsync(purch_requisitionnote prn)
        {

            EmailMessageObj em = new EmailMessageObj { ToAddresses = new List<EmailAddressObj>() };

            var staffList = await _serverRequest.GetAllStaffAsync();
            if(staffList.Count() > 0)
            {
                var intiatingStaff = staffList.FirstOrDefault(r => r.staffId == prn.StaffId);
                if(intiatingStaff != null)
                {
                    //var path = $"#/main/lpos";

                    //var path2 = $"{_uRIs.SelfClient}/{path}";
                    em.Subject = "PRN APPROVED";
                    em.Content = $"The Request for {prn.Description} has been approved ";
                    em.FromAddresses = new List<EmailAddressObj>();
                    var to = new EmailAddressObj { Address = intiatingStaff.Email, Name = intiatingStaff.firstName };
                    em.ToAddresses.Add(to);
                    em.SendIt = true;
                    em.SaveIt = false;
                    em.Template = (int)EmailTemplate.Advert;

                    await _serverRequest.SendMessageAsync(em);
                } 
            }  
        }
    }
}
