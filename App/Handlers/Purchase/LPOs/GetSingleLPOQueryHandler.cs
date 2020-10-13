using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Repository.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase.LPOs
{
    public class GetSingleLPOQuery : IRequest<LPORespObj>
    {
        public int LPOId { get; set; }
        public class GetSingleLPOQueryHandler : IRequestHandler<GetSingleLPOQuery, LPORespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly ISupplierRepository _supRepo; 
            public GetSingleLPOQueryHandler(IPurchaseService purchaseService, ISupplierRepository supplierRepository)
            {
                _supRepo = supplierRepository;
                _repo = purchaseService;
            }
            private async Task<List<cor_taxsetup>> GetTaxSetupSupplierTypeWithAsync(int supplierId)
            {
                var supplier = await _supRepo.GetSupplierAsync(supplierId);
                if (supplier != null)
                {
                    var supplierType = await _supRepo.GetSupplierTypeAsync(supplier.SupplierTypeId);
                    if (supplierType != null)
                    {
                        var TaxIds = supplierType.TaxApplicable.Split(',').Select(int.Parse).Distinct();
                        var ListOfTaxapplicable = new List<cor_taxsetup>();
                        foreach (var tId in TaxIds)
                        {
                            var taxApplicable = await _supRepo.GetTaxSetupAsync(tId);
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
            private async Task<List<cor_taxsetup>> GetTaxSetupWithAsync(string TaxApplicable)
            {
                var TaxIds = TaxApplicable.Split(',').Select(int.Parse).Distinct();
                if (TaxIds.Count() > 0)
                {
                    var ListOfTaxapplicable = new List<cor_taxsetup>();
                    foreach (var tId in TaxIds)
                    {
                        var taxApplicable = await _supRepo.GetTaxSetupAsync(tId);
                        if (taxApplicable != null)
                        {
                            ListOfTaxapplicable.Add(taxApplicable);
                        }
                    }
                    return ListOfTaxapplicable;
                }

                return new List<cor_taxsetup>();
            }

           

            public async Task<LPORespObj> Handle(GetSingleLPOQuery request, CancellationToken cancellationToken)
            {
                var response = new LPORespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
                try
                {
                    var d = await _repo.GetLPOsAsync(request.LPOId);

                    var paymentTerms = await _repo.GetPaymenttermsAsync();
                    var result = new List<LPOObj>();
                    if (d != null)
                    {
                        var LPO = new LPOObj
                        {
                            SupplierAddress = d.Address,
                            ApprovalStatusId = d.ApprovalStatusId,
                            DeliveryDate = d.DeliveryDate,
                            Description = d.Description,
                            LPONumber = d.LPONumber,
                            Name = d.Name,
                            PLPOId = d.PLPOId,
                            SupplierId = d.SupplierIds,
                            Tax = d.Tax,
                            Total = d.Total,
                            AmountPayable = d.AmountPayable,
                            BidAndTenderId = d.BidAndTenderId,
                            GrossAmount = d.GrossAmount,
                            JobStatus = d.JobStatus,
                            JobStatusName = Convert.ToString((JobProgressStatus)d.JobStatus),
                            RequestDate = d.RequestDate,
                            SupplierNumber = d.SupplierNumber,
                            Location = d.Address,
                            Quantity = d.Quantity,
                            WorkflowToken = d.WorkflowToken,
                            ServicetermsId = d.ServiceTerm,
                            Taxes = string.IsNullOrEmpty(d.Taxes) ? GetTaxSetupSupplierTypeWithAsync(d.WinnerSupplierId).Result.Select(w => new TaxObj
                            {
                                ShouldExcept = true,
                                TaxID = w.TaxSetupId,
                                TaxName = w.TaxName,
                                Percentage = w.Percentage,
                                SubGL = w.SubGL,
                                Type = w.Type,

                            }).ToList() : GetTaxSetupWithAsync(d.Taxes).Result.Select(w => new TaxObj
                            {
                                ShouldExcept = false,
                                TaxID = w.TaxSetupId,
                                TaxName = w.TaxName,
                                Percentage = w.Percentage,
                                SubGL = w.SubGL,
                                Type = w.Type,

                            }).ToList(),
                            PaymentTerms = paymentTerms.Where(r => r.BidAndTenderId == d.BidAndTenderId).Select(c => new PaymentTermsObj
                            {
                                BidAndTenderId = c.BidAndTenderId,
                                Comment = c.Comment,
                                Completion = c.Completion,
                                Amount = c.Amount,
                                NetAmount = c.NetAmount,
                                Payment = c.Payment,
                                PaymentStatus = c.PaymentStatus,
                                PaymentTermId = c.PaymentTermId,
                                Phase = c.Phase,
                                ProjectStatusDescription = c.ProjectStatusDescription,
                                Status = c.Status,
                                PhaseTax = c.TaxPercent,
                                PaymentStatusName = Convert.ToString((PaymentStatus)c.PaymentStatus),
                                ProposedBy = c.ProposedBy,
                                StatusName = Convert.ToString((JobProgressStatus)c.Status)
                            }).ToList(),
                        };
                        result.Add(LPO);
                    }

                    return new LPORespObj
                    {
                        LPOs = result,
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = true,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = result.Count() > 0 ? null : "Search Complete! No Record found"
                            }
                        }
                    };
                }
                catch (Exception ex)
                {
                    return response;
                }
            }
        }
    }
}


