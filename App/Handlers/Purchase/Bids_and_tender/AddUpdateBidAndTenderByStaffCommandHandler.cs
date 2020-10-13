using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http; 
using Puchase_and_payables.Contracts.Commands.Purchase; 
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class AddUpdateBidAndTenderByStaffCommandHandler : IRequestHandler<AddUpdateBidAndTenderByStaffCommand, BidAndTenderRegRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly ILoggerService _logger;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly IHttpContextAccessor _accessor;
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _env;
        public AddUpdateBidAndTenderByStaffCommandHandler(
            IPurchaseService purchaseService,
            ILoggerService loggerService,
            IWebHostEnvironment webHostEnvironment,
            DataContext dataContext,
            IHttpContextAccessor httpContextAccessor,
            IIdentityServerRequest serverRequest)
        {
            _logger = loggerService;
            _env = webHostEnvironment;
            _accessor = httpContextAccessor;
            _repo = purchaseService;
            _serverRequest = serverRequest;
            _dataContext = dataContext;
        }
        public async Task<BidAndTenderRegRespObj> Handle(AddUpdateBidAndTenderByStaffCommand request, CancellationToken cancellationToken)
        {
            var response = new BidAndTenderRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
            try
            {
                var user = await _serverRequest.UserDataAsync();
                if (user == null)
                {
                    response.Status.Message.FriendlyMessage = "Unable To Process This User";
                    return response;
                }
                cor_bid_and_tender bidAndTenderObj = new cor_bid_and_tender();
                var supplierAdvert =  _dataContext.cor_bid_and_tender.FirstOrDefault(q => q.SupplierId == request.SupplierId && q.LPOnumber.Trim().ToLower() == request.LPONumber.Trim().ToLower());
            
                 using(var trans = _dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (supplierAdvert != null)
                        {
                            request.SupplierId = supplierAdvert.SupplierId;
                            request.BidAndTenderId = supplierAdvert.BidAndTenderId;
                            bidAndTenderObj = BuildBidAndTenderObject(request);
                            await _repo.AddUpdateBidAndTender(bidAndTenderObj);
                        }
                        else
                        {
                            bidAndTenderObj = BuildBidAndTenderObject(request);
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {

                        trans.Rollback();
                        throw ex;
                    }
                    finally { trans.Dispose(); }
                    
                }
               

                response.Status.IsSuccessful = true;
                response.BidAndTenderId = request.BidAndTenderId;
                response.Status.Message.FriendlyMessage = "Successful";
                return response;

            }
            catch (Exception ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new BidAndTenderRegRespObj
                {

                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Unable to process item",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        }
        private cor_bid_and_tender BuildBidAndTenderObject(AddUpdateBidAndTenderByStaffCommand request)
        {
            List<cor_paymentterms> childItemList = new List<cor_paymentterms>();
            var terms = _dataContext.cor_paymentterms.Where(w => w.BidAndTenderId == request.BidAndTenderId).ToList();
            if(terms.Count() > 0)
            {
                _dataContext.cor_paymentterms.RemoveRange(terms);
                _dataContext.SaveChangesAsync();
            }
            if (request.Paymentterms.Count() > 0)
            {
                foreach(var cI in request.Paymentterms)
                {
                    var childItem = new cor_paymentterms
                    {
                        Comment = cI?.Comment,
                        Completion = cI?.Completion ?? 0,
                        Payment = cI?.Payment??0,
                        Phase = cI?.Phase??0,
                        PaymentTermId = cI?.PaymentTermId??0,
                        ProjectStatusDescription = cI?.ProjectStatusDescription,
                        BidAndTenderId = request.BidAndTenderId,
                        ProposedBy = (int)Proposer.SUPPLIER,
                        Amount = cI.Amount
                    };
                    childItemList.Add(childItem);
                }
            }

            var bid =  _dataContext.cor_bid_and_tender.FirstOrDefault(w => w.SupplierId == request.SupplierId && w.LPOnumber.Trim().ToLower() == request.LPONumber.Trim().ToLower());
            if (!string.IsNullOrEmpty(bid.SelectedSuppliers))
            {
                bid.BidAndTenderId = 0;
            }
            else
            {
                bid.BidAndTenderId = request.BidAndTenderId;
            }
            bid.Suppliernumber = request.Suppliernumber;
            bid.SupplierName = request.SupplierName;
            bid.RequestingDepartment = request.RequestingDepartment;
            bid.RequestDate = request.RequestDate;
            bid.ProposedAmount = request.ProposedAmount;
            bid.AmountApproved = request.AmountApproved; 
            bid.DateSubmitted = DateTime.Now;
            bid.DecisionResult = request.DecisionResult;
            bid.DescriptionOfRequest = request.DescriptionOfRequest;
            bid.Location = request.Location;
            bid.LPOnumber = request.LPONumber;
            bid.SupplierId = request.SupplierId;
            bid.ApprovalStatusId = (int)ApprovalStatus.Pending;
            bid.Paymentterms = childItemList;
            bid.Quantity = request.Quantity;
            bid.Total = request.Total;
            bid.ExpectedDeliveryDate = request.ExpectedDeliveryDate;
            return bid;
        }
    }

}
