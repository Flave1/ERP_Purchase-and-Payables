using GODP.APIsContinuation.Handlers.Supplier;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puchase_and_payables.Contracts.Queries.Supplier.Web;
using Puchase_and_payables.Contracts.V1;
using Puchase_and_payables.Handlers.Purchase;
using Puchase_and_payables.Handlers.Purchase.Invoice;
using Puchase_and_payables.Handlers.Purchase.LPOs;
using System.Threading.Tasks;

namespace Puchase_and_payables.Controllers.V1
{
    [Authorize]
    public class OnWebSupplierController : Controller
    {
        private readonly IMediator _mediator;
        public OnWebSupplierController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(ApiRoutes.OnWebSupplierEndpoints.GET_THIS_SUPPLIER_DATA)]
        public async Task<ActionResult> GET_THIS_SUPPLIER_DATA()
        {
            var query = new OnWebSupplierGetQuery();
            return Ok(await _mediator.Send(query));
        }
        
        [HttpGet(ApiRoutes.OnWebSupplierEndpoints.GET_SUPPLIER_BIDs)]
        public async Task<ActionResult> GET_SUPPLIER_BIDs([FromQuery] GetPreviousBidAndTenderQuery query)
        { 
            return Ok(await _mediator.Send(query));
        }
        
        [HttpGet(ApiRoutes.OnWebSupplierEndpoints.GET_SUPPLIER_LPO)]
        public async Task<ActionResult> GET_SUPPLIER_LPO()
        {
            var query = new GetAllSupplierPOQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.Purchases.RESPOND_TO_LPO)]
        public async Task<ActionResult> RESPOND_TO_LPO([FromBody] RespondToLPOCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }
        [HttpGet(ApiRoutes.OnWebSupplierEndpoints.GET_ALL_COUNTRY)]
        public async Task<ActionResult> GET_ALL_COUNTRY()
        {
            var query = new GetAllCountriesQuery();
            return Ok(await _mediator.Send(query));
        }
        [HttpGet(ApiRoutes.OnWebSupplierEndpoints.GET_ALL_DOCUMENTYPE)]
        public async Task<ActionResult> GET_ALL_DOCUMENTYPE()
        {
            var query = new GetAllDocumenttypesQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.Purchases.UPLOAD_PROPOSAL)]
        public async Task<ActionResult> UPLOAD_PROPOSAL([FromForm] UploadProposalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }


        [HttpGet(ApiRoutes.Purchases.DOWNLOAD_PROPOSAL)]
        public async Task<ActionResult> DOWNLOAD_PROPOSAL([FromQuery] DownloadProposalQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.Purchases.SUPPLIER_UPDATE_PROPOSAL)]
        public async Task<ActionResult> SUPPLIER_UPDATE_PROPOSAL([FromForm] UpdateProposalBySupplierCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp); ;
        }
        [HttpGet(ApiRoutes.OnWebSupplierEndpoints.GET_BANKS)]
        public async Task<ActionResult> GET_BANKS()
        {
            var query = new GetBanskQuery();
            return Ok(await _mediator.Send(query));
        }
    }
}
