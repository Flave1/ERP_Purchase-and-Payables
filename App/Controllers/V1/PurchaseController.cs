using GODP.APIsContinuation.Handlers.Supplier;
using GODPAPIs.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.V1;
using Puchase_and_payables.Handlers.Purchase;
using Puchase_and_payables.Handlers.Purchase.LPOs;
using Puchase_and_payables.Handlers.Purchase.Payment_proposals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Controllers.V1
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly IMediator _mediator;
        public PurchaseController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost(ApiRoutes.Purchases.ADD_PURCHASE_REQ_NOTE)]
        public async Task<ActionResult> ADD_PURCHASE_REQ_NOTE([FromBody] AddUpdateRequisitionNoteCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        [HttpPost(ApiRoutes.Purchases.STAFF_PURCHASE_REQ_NOTE_APPROVAL)]
        public async Task<ActionResult> APPROVAL_REQUEST([FromBody] PRNStaffApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }
        [HttpGet(ApiRoutes.Purchases.GET_SUPPLIERS_BY_PRN)]
        public async Task<ActionResult> GET_SUPPLIERS_BY_PRN([FromQuery] GetSuppliersByPRNQUery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpGet(ApiRoutes.Purchases.GET_PRN_AWAITING_APPROVALS)]
        public async Task<ActionResult> GET_PRN_AWAITING_APPROVALS()
        {
            var query = new GetAllPRNAwaitingApprovalQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpGet(ApiRoutes.Purchases.GET_BID_AWAITING_APPROVALS)]
        public async Task<ActionResult> GET_BID_AWAITING_APPROVALS()
        {
            var query = new GetBidAndTenderAwaitingApprovalQuery();
            return Ok(await _mediator.Send(query));
        }
         
        [HttpGet(ApiRoutes.Purchases.GET_SINGLE_PRN_DETAIL_APPROVALS)]
        public async Task<ActionResult> GET_SINGLE_PRN_DETAIL_APPROVALS([FromQuery] GetPRNAwaitingApprovalQuery query)
        {
            var resp = await _mediator.Send(query);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        [HttpGet(ApiRoutes.Purchases.GET_ALL_LPOS)]
        public async Task<ActionResult> GET_ALL_LPOS()
        {
            var query = new GetAllLPOQuery();
            return Ok(await _mediator.Send(query));
        }

        
        [HttpGet(ApiRoutes.Purchases.GET_ALL_BID_AND_TENDERS)]
        public async Task<ActionResult> GET_ALL_BID_AND_TENDERS([FromQuery] GetBidAndTenderQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.Purchases.ADD_UPDATE_BID_AND_TENDERS)]
        public async Task<ActionResult> ADD_UPDATE_BID_AND_TENDERS([FromBody] AddUpdateBidAndTenderCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }
        [HttpPost(ApiRoutes.Purchases.STAFF_APPROVAL_BID_AND_TENDER)]
        public async Task<ActionResult> STAFF_APPROVAL_BID_AND_TENDER([FromBody] BidandTenderStaffApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        [HttpPost(ApiRoutes.Purchases.SEND_BID_TO_APPROVAL)]
        public async Task<ActionResult> SEND_BID_TO_APPROVAL([FromBody] SendSupplierBidAndTenderToApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        [HttpGet(ApiRoutes.Purchases.GET_BID_AND_TENDER)]
        public async Task<ActionResult> GET_BID_AND_TENDER([FromQuery] GetBidAndTenderByIdQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet(ApiRoutes.Purchases.GET_ALL_BID_AND_TENDER)]
        public async Task<ActionResult> GET_ALL_BID_AND_TENDER()
        {
            var query = new GetAvailableBidsQuery();
            return Ok(await _mediator.Send(query));
        }
 

        [HttpPost(ApiRoutes.Purchases.LPO_STAFF_APPRPVAL)]
        public async Task<ActionResult> LPO_STAFF_APPRPVAL([FromBody] LPOStaffApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        //[HttpPost(ApiRoutes.Purchases.UPDATE_PAYMENT)]
        //public async Task<ActionResult> UPDATE_PAYMENT([FromBody] UpdatePaymentTermsCommand command)
        //{
        //    var resp = await _mediator.Send(command);
        //    if (!resp.Status.IsSuccessful)
        //        return BadRequest(resp);
        //    return Ok(resp);
        //}

        [HttpGet(ApiRoutes.Purchases.GET_ALL_INVOICE)]
        public async Task<ActionResult> GET_ALL_INVOICE()
        {
            var query = new GetListInvoicesQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpGet(ApiRoutes.Purchases.GET_INVOICE_DETAIL)]
        public async Task<ActionResult> GET_INVOICE_DETAIL([FromQuery] GetInvoiceDetailQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpGet(ApiRoutes.Purchases.GET_LPO_AWAITING_APPROAL)]
        public async Task<ActionResult> GET_LPO_AWAITING_APPROAL( )
        {
            var query = new GetLPOAwaitingApprovalQuery();
            return Ok(await _mediator.Send(query));
        }

        
        [HttpGet(ApiRoutes.Purchases.GET_ALL_PRNs)]
        public async Task<ActionResult> GET_ALL_PRNs()
        {
            var query = new GetAllPRNQuery();
            return Ok(await _mediator.Send(query));
        }

        
        [HttpPost(ApiRoutes.Purchases.SEND_PRN_FOR_APPROVALS)]
        public async Task<ActionResult> SEND_PRN_FOR_APPROVALS([FromBody] SendPRNToApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }
         

        [HttpGet(ApiRoutes.Purchases.GET_ALL_PAYMENTTERMS)]
        public async Task<ActionResult> GET_ALL_PAYMENTTERMS([FromQuery] GetPaymenttermsByIdsQuery query)
        {
            return Ok(await _mediator.Send(query));  
        }

        
        [HttpGet(ApiRoutes.Purchases.GET_SINGLE_BIDAND_TENDER)]
        public async Task<ActionResult> GET_SINGLE_BIDAND_TENDER([FromQuery] GetSingleBidAndTenderQuery query)
        { 
            return Ok(await _mediator.Send(query));
        }
        
        [HttpPost(ApiRoutes.Purchases.SAVE_UPDATE_PAYMENTTERMS)]
        public async Task<ActionResult> SAVE_UPDATE_PAYMENTTERMS([FromBody] SaveUpdatePaymentTermsCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }
         
        [HttpGet(ApiRoutes.Purchases.GET_SINGLE_LPO)]
        public async Task<ActionResult> GET_SINGLE_LPO([FromQuery] GetSingleLPOQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.Purchases.SEND_LPO_TO_APPROVALS)]
        public async Task<ActionResult> SEND_LPO_TO_APPROVALS([FromBody] SendLPOToApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }


        [HttpPost(ApiRoutes.Purchases.UPDATE_LPO_TO_APPROVALS)]
        public async Task<ActionResult> UPDATE_LPO_TO_APPROVALS([FromBody] SaveUpdateLPOCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        [HttpPost(ApiRoutes.Purchases.DELETE_PROPOSAL)]
        public async Task<ActionResult> DELETE_PROPOSAL([FromBody] DeletePaymentProposalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }


        [HttpPost(ApiRoutes.Purchases.SAVE_PROPOSAL_PHASE)]
        public async Task<ActionResult> SAVE_PROPOSAL_PHASE([FromForm] UpdatePaymentProposalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }

        [HttpPost(ApiRoutes.Purchases.REQUEST_PAYMENT)]
        public async Task<ActionResult> REQUEST_PAYMENT([FromBody] RequestforPaymentCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }

        [HttpPost(ApiRoutes.Purchases.SEND_PROPOSAL_PHASE_TO_APPROVAL)]
        public async Task<ActionResult> SEND_PROPOSAL_PHASE_TO_APPROVAL([FromBody] SendPaymentInvoiceToApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }

        
        [HttpPost(ApiRoutes.Purchases.update_ppe_lpo)]
        public async Task<ActionResult> update_ppe_lpo()
        {
            var command = new RefreshPPELPOCommand();
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }

        [HttpPost(ApiRoutes.Purchases.STAFF_PAYMENT)]
        public async Task<ActionResult> STAFF_PAYMENT([FromBody] PaymentRequestStaffApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        [HttpGet(ApiRoutes.Purchases.PAYMENT_WAITING_APPROVALS)]
        public async Task<ActionResult> PAYMENT_WAITING_APPROVALS()
        {
            var query = new GetPaymentProposalAwaitingApprovalQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpGet(ApiRoutes.Purchases.GET_ALL_APPROVED_PRNS)]
        public async Task<ActionResult> GET_ALL_APPROVED_PRNS()
        {
            var query = new GetApprovedPRNQuery();
            return Ok(await _mediator.Send(query));
        }
        
        [HttpGet(ApiRoutes.Purchases.GET_NOT_BIDDEN_ADVERTS)]
        public async Task<ActionResult> GET_NOT_BIDDEN_ADVERTS()
        {
            var query = new GetSupplierAdvertsNotBiddenForQuery();
            return Ok(await _mediator.Send(query));
        }
      

        [HttpGet(ApiRoutes.Purchases.DOWNLOAD_COMPLETION_CERT)]
        public async Task<ActionResult> DOWNLOAD_COMPLETION_CERT([FromQuery] DownloadCertificateQuery query)
        {
            var resp = await _mediator.Send(query);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }

        [HttpPost(ApiRoutes.Purchases.BID_BY_STAFF)]
        public async Task<ActionResult> BID_BY_STAFF([FromBody] AddUpdateBidAndTenderByStaffCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }
        
    }
}
