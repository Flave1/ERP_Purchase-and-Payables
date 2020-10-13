using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puchase_and_payables.Contracts.Response.Report;
using Puchase_and_payables.Contracts.V1;
using Puchase_and_payables.Handlers.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Controllers.V1
{ 
    public class ReportController : Controller
    {
        private readonly IMediator _mediator;
        public ReportController(IMediator  mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet(ApiRoutes.Report.AGINGANALYSIS_TABLE)]
        public async Task<ActionResult> AGINGANALYSIS_TABLE([FromQuery] AgingAnalysisTableQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        

        [HttpGet(ApiRoutes.Report.LPO_REPORT)]
        public async Task<ActionResult> LPO_REPORT([FromQuery] LPOReportQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpGet(ApiRoutes.Report.DASHBOARD_COUNTS)]
        public async Task<ActionResult> DASHBOARD_COUNTS()
        {
            var query = new DashboardCountQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpGet(ApiRoutes.Report.PAYABLE_DAYS)]
        public async Task<ActionResult> PAYABLE_DAYS()
        {
            var query = new PayableDaysCalcultion();
            return Ok(await _mediator.Send(query));
        }
        [HttpGet(ApiRoutes.Report.DASHBOARD_AGE_ANALYSIS)]
        public async Task<ActionResult> DASHBOARD_AGE_ANALYSIS()
        {
            var query = new AgingAnalysisGraphQuery();
            return Ok(await _mediator.Send(query));
        }

        
        [HttpGet(ApiRoutes.Report.PROJECT_STATUS)]
        public async Task<ActionResult> PROJECT_STATUS()
        {
            var query = new ProjectStatusQuery();
            return Ok(await _mediator.Send(query));
        }
        
        [HttpGet(ApiRoutes.Report.PAYABLEDAYS_ANALYSIS)]
        public async Task<ActionResult> PAYABLEDAYS_ANALYSIS()
        {
            var query = new PayableDaysTrendAnalysis();
            return Ok(await _mediator.Send(query));
        }
        
        [HttpGet(ApiRoutes.Report.PURCHASES)]
        public async Task<ActionResult> PURCHASES()
        {
            var query = new PurchasesTrendAnalysis();
            return Ok(await _mediator.Send(query));
        }

        [HttpGet(ApiRoutes.Report.PURCH_AND_PAYABLES_REPORT)]
        public async Task<ActionResult> PURCH_AND_PAYABLES_REPORT([FromQuery] PurchaseAndPayablesReportQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        
    }
}
