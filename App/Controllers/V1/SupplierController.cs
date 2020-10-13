using GODP.APIsContinuation.Handlers.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Commands.Supplier;
using GODPAPIs.Contracts.Queries;
using GOSLibraries;
using GOSLibraries.Enums;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puchase_and_payables.Contracts.Commands.Supplier;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Commands.Supplier.setup;
using Puchase_and_payables.Contracts.Queries.Details;
using Puchase_and_payables.Contracts.Queries.Supplier;
using Puchase_and_payables.Contracts.V1;
using Puchase_and_payables.Handlers.Requirement;
using Puchase_and_payables.Handlers.Supplier.Approvals;
using Puchase_and_payables.Handlers.Supplier.Settup;
using Puchase_and_payables.Handlers.Supplier.Settup.Uploads_Downloads;
using Puchase_and_payables.Handlers.Supplier.Uploads_Downloads;
using System.Threading.Tasks;

namespace Puchase_and_payables.Controllers.V1
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SupplierController : Controller
    {
        private readonly IMediator _mediator; 
        public SupplierController(IMediator mediator)
        {
            _mediator = mediator; 
        }
         
        [ERPActivity(Action = UserActions.Add, Activity = 33)]
        [HttpPost(ApiRoutes.SupplierEndpoints.ADD_UPDATE_TERMS)]
        public async Task<ActionResult> ADD_UPDATE_TERMS([FromBody] AddUpdateServiceTermCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [ERPActivity(Action = UserActions.Add, Activity = 34)]
        [HttpPost(ApiRoutes.SupplierEndpoints.ADD_UPDATE_SUPPLIER_TYPE)]
        public async Task<ActionResult> ADD_UPDATE_SUPPLIER_TYPE([FromBody] AddUpdateSuppliertypeCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [ERPActivity(Action = UserActions.Add, Activity = 32)]
        [HttpPost(ApiRoutes.SupplierEndpoints.ADD_UPDATE_TASK_SETUP)]
        public async Task<ActionResult> ADD_UPDATE_TASK_SETUP([FromBody] AddUpdateTaxsetupCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_TERMS)]
        public async Task<ActionResult> GET_ALL_TERMS()
        {
            var query = new GetAllServiceTermsQuery();
            return Ok(await _mediator.Send(query));
        }
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_SUPPLIER_TYPE)]
        public async Task<ActionResult> GET_ALL_SUPPLIER_TYPE()
        {
            var query = new GetAllSupplierTypeQuery();
            return Ok(await _mediator.Send(query));
        }
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_TASK_SETUP)]
        public async Task<ActionResult> GET_ALL_TASK_SETUP()
        {
            var query = new GetAllTaxSetupQuery();
            return Ok(await _mediator.Send(query));
        }
        [ERPActivity(Action = UserActions.View, Activity = 33)]
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_TERMS)]
        public async Task<ActionResult> GET_TERMS([FromQuery] GetServiceTermsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        [ERPActivity(Action = UserActions.View, Activity = 34)]
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER_TYPE)]
        public async Task<ActionResult> GET_SUPPLIER_TYPE([FromQuery] GetSupplierTypeQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [ERPActivity(Action = UserActions.View, Activity = 32)]
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_TASK_SETUP)]
        public async Task<ActionResult> GET_TASK_SETUP([FromQuery] GetTaxSetupQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_SUPPLIERS)]
        public async Task<ActionResult> GetAllSuppliers()
        {
            var query = new GetAllSupplierQuery();
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER)]
        public async Task<ActionResult> GetSupplier([FromQuery]GetSupplierQuery query)
        {
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_SUPPLIER_AUTHORIZATIONS)]
        public async Task<ActionResult> GetAllSupplierAuths([FromQuery] GetAllSupplierAuthorizationQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER_AUTHORIZATION)]
        public async Task<ActionResult> GetSupplier([FromQuery] GetSupplierAuthorization query)
        {
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_SUPPLIER_BUSINESS_OWNER)]
        public async Task<ActionResult> GetAllSupplierBus([FromQuery] GetAllSupplierBusinessOwnerQuery query)
        { 
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER_BUSINESS_OWNER)]
        public async Task<ActionResult> GetSupplierBus([FromQuery] GetSupplierBusinessOwnerQuery query)
        {
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_SUPPLIER_DOCUMENTS)]
        public async Task<ActionResult> GetAllSupplierDocs([FromQuery] GetAllSupplierDocumentQuery query)
        { 
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER_DOCUMENT)]
        public async Task<ActionResult> GetSupplierBus([FromQuery] GetSupplierDocumentQuery query)
        {
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_TOP_CLIENTS)]
        public async Task<ActionResult> GetAllSupplierTopClients([FromQuery] GetAllSupplierTopClientQuery query)
        {
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER_TOP_CLIENT)]
        public async Task<ActionResult> GetSupplierClient([FromQuery] GetSupplierTopClientQuery query)
        {
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_TOP_SUPPLIERS)]
        public async Task<ActionResult> GetAllSupplierTopSuppliers([FromQuery] GetAllSupplierTopSupplierQuery query)
        {
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER_TOP_SUPPLIER)]
        public async Task<ActionResult> GetSupplierTopSupplier([FromQuery] SupplierTopSupplierQuery query)
        {
            var response = await _mediator.Send(query);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPDATE_SUPPLIER)]
        public async Task<ActionResult> UpdateSuppliers([FromBody] UpdateSupplierCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [Produces("application/json")]
        [HttpPost(ApiRoutes.SupplierEndpoints.UPDATE_SUPPLIER_AUTHORIZATION)]
        public async Task<ActionResult> UpdateSupAuth([FromForm] UpdateSupplierAuthorizationCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPDATE_SUPPLIER_BUSINESS_OWNER)]
        public async Task<ActionResult> UpdateSupAuth([FromForm] UpdateSupplierBuisnessOwnerCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPDATE_SUPPLIER_DOCUMENT)]
        public async Task<ActionResult> UpdateSupDoc([FromForm] UpdateSupplierDocumentCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPDATE_SUPPLIER_TOP_CLIENT)]
        public async Task<ActionResult> UpdateTopClient([FromBody] UpdateSupplierTopClientCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPDATE_SUPPLIER_TOP_SUPPLIER)]
        public async Task<ActionResult> UpdateTopSup([FromBody] UpdateSupplierTopSupplierCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER)]
        public async Task<ActionResult> DeleteSupplier([FromBody] DeleteSupplierCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_AUTHORIZATION)]
        public async Task<ActionResult> DeleteSupplierAuth([FromBody] DeleteSupplierAuthorizationCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_BUSINESS_OWNER)]
        public async Task<ActionResult> DeleteBusOwn([FromBody] DeleteSupplierBuisnessOwnerCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_DOCUMENT)]
        public async Task<ActionResult> DeleteSupDoc([FromBody] DeleteSupplierDocumentCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_TOP_CLIENT)]
        public async Task<ActionResult> DeleteSupDoc([FromBody] DeleteSupplierTopClientCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_TOP_SUPPLIER)]
        public async Task<ActionResult> DeleteSupDoc([FromBody] DeleteSupplierTopSupplierCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }




        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_TYPE)]
        public async Task<ActionResult> DELETE_SUPPLIER_TYPE([FromBody] DeleteSupplierTypeCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_BANK_DETAILS)]
        public async Task<ActionResult> DELETE_SUPPLIER_BANK_DETAILS([FromBody] DeleteSupplierBankDetailsCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_FINANCIAL_DETAILS)]
        public async Task<ActionResult> DELETE_SUPPLIER_FINANCIAL_DETAILS([FromBody] DeleteSupplierFinancialDetalsCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_IDENTIFICATION)]
        public async Task<ActionResult> DELETE_SUPPLIER_IDENTIFICATION([FromBody] DeleteSupplierIdentificationCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_SERVICE_TERMS)]
        public async Task<ActionResult> DELETE_SUPPLIER_SERVICE_TERMS([FromBody] DeleteSupplierServiceTermsCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.SupplierEndpoints.DELETE_SUPPLIER_TAX_SETUP)]
        public async Task<ActionResult> DELETE_SUPPLIER_TAX_SETUP([FromBody] DeleteSupplierTaxSetupCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_AWAITING_APPROVALS)]
        public async Task<ActionResult> GET_AWAITING_APPROVALS()
        {
            var query = new GetAllSupplierDataAwaitingApprovalQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.APPROVAL_REQUEST)]
        public async Task<ActionResult> APPROVAL_REQUEST([FromBody] SupplierStaffApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.APPROVAL_DETAILS)]
        public async Task<ActionResult> APPROVAL_DETAILS([FromQuery] GetCurrentTargetApprovalDetailQuery query)
        {
            var resp = await _mediator.Send(query);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        //..
        [HttpPost(ApiRoutes.SupplierEndpoints.ADD_UPPDATE_SUPPLIER_BANK_DETA)]
        public async Task<ActionResult> ADD_UPPDATE_SUPPLIER_BANK_DETA([FromBody] AddUpdateBankAccountdetailCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER_BANK_DETA)]
        public async Task<ActionResult> GET_SUPPLIER_BANK_DETA([FromQuery] GetBankAccountdetailQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_SUPPLIER_BANK_DETA)]
        public async Task<ActionResult> GET_ALL_SUPPLIER_BANK_DETA([FromQuery] GetAllBankAccountdetailQuery query)
        {
            var resp = await _mediator.Send(query);
            return Ok(resp);
        }


        [HttpPost(ApiRoutes.SupplierEndpoints.ADD_UPPDATE_SUPPLIER_FINANCIAL_DETA)]
        public async Task<ActionResult> ADD_UPPDATE_SUPPLIER_FINANCIAL_DETA([FromBody] AddUpdateSupplierFinancialDetalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUPPLIER_FINANCIAL_DETA)]
        public async Task<ActionResult> GET_SUPPLIER_FINANCIAL_DETA([FromQuery] GetFinancialDetailQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_SUPPLIER_FINANCIAL_DETA)]
        public async Task<ActionResult> GET_ALL_SUPPLIER_FINANCIAL_DETA([FromQuery] GetAllFinancialDetailsQuery query)
        {
            var resp = await _mediator.Send(query); 
            return Ok(resp);
        }

        
        [HttpPost(ApiRoutes.SupplierEndpoints.GO_THROUGH_APPROVAL)]
        public async Task<ActionResult> GO_THROUGH_APPROVAL([FromBody] GothroughApprovalCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_LIST_OF_APPROVAED_SUPPLIERS)]
        public async Task<ActionResult> GET_LIST_OF_APPROVAED_SUPPLIERS()
        {
           var query = new GetListOfApprovedSuppliersQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.ADD_UPDATE_SUPP_IDENTIFICATION)]
        public async Task<ActionResult> ADD_UPDATE_SUPP_IDENTIFICATION([FromBody] AddUpdateSupplierIdentificationCommand command)
        {
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_SUP_IDENTIFICATION)]
        public async Task<ActionResult> GET_ALL_SUP_IDENTIFICATION([FromQuery] GetAllSupplierIdentificationQuery query)
        {
            var resp = await _mediator.Send(query);
            return Ok(resp);
        }
        [HttpGet(ApiRoutes.SupplierEndpoints.GET_SUP_IDENTIFICATION)]
        public async Task<ActionResult> GET_SUP_IDENTIFICATION([FromQuery] GetSupplierIdentificationQuery query)
        {
            var resp = await _mediator.Send(query);
            return Ok(resp);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GENERATE_TAXSETUP_FILE)]
        public async Task<ActionResult> EXPORT_TAXSETUP_FILE()
        {
            var command = new DownloadTaxSetupQuery();
            return Ok(await _mediator.Send(command));
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPLOAD_TAXSETUP_FILE)]
        public async Task<ActionResult> IMPORT_TAXSETUP_FILE()
        {
            var command = new UploadTaxSetupCommand();
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        } 

        [HttpGet(ApiRoutes.SupplierEndpoints.GENERATE_SERVICETERM_FILE)]
        public async Task<ActionResult> GENERATE_SERVICETERM_FILE()
        {
            var command = new DownloadServiceTermsQuery();
            return Ok(await _mediator.Send(command));
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPLOAD_SERVICETERM_FILE)]
        public async Task<ActionResult> UPLOAD_SERVICETERM_FILE()
        {
            var command = new UploadServiceTermsCommand();
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }
         
        [HttpGet(ApiRoutes.SupplierEndpoints.GENERATE_SUPPLIERTYP_FILE)]
        public async Task<ActionResult> GENERATE_SUPPLIERTYP_FILE()
        {
            var command = new DownloadSupplierTypeQuery();
            return Ok(await _mediator.Send(command));
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPLOAD_SUPPLIERTYP_FILE)]
        public async Task<ActionResult> UPLOAD_SUPPLIERTYP_FILE()
        {
            var command = new UploadSupplierTypeCommand();
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

         
        [HttpGet(ApiRoutes.SupplierEndpoints.GENERATE_SUPPLIERINFORMATION_FILE)]
        public async Task<ActionResult> GENERATE_SUPPLIERINFORMATION_FILE()
        {
            var query = new DownloadSupplierInformationQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.UPLOAD_SUPPLIERINFORMATION_FILE)]
        public async Task<ActionResult> UPLOAD_SUPPLIERINFORMATION_FILE()
        {
            var command = new UploadSupplierInformationCommand();
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        }

        [HttpGet(ApiRoutes.SupplierEndpoints.GET_ALL_PENDING_SUPPLIERS)]
        public async Task<ActionResult> GET_ALL_PENDING_SUPPLIERS()
        {
            var query = new GetAllPendingSupplierQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpPost(ApiRoutes.SupplierEndpoints.SUPPLIER_MULTIPLE_APPROVE)]
        public async Task<ActionResult> SUPPLIER_MULTIPLE_APPROVE([FromBody] SupplierMultipleStaffApprovalCommand command)
        { 
            var resp = await _mediator.Send(command);
            if (!resp.Status.IsSuccessful)
                return BadRequest(resp);
            return Ok(resp);
        } 
    } 
}
