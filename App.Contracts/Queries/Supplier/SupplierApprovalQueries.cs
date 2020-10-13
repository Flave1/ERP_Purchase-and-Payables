using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Queries.Supplier
{
	 

	public class GetAllSupplierDataAwaitingApprovalQuery : IRequest<SupplierRespObj> { }
	
}
