using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json; 
using Puchase_and_payables.Contracts.Queries.Supplier;
using Puchase_and_payables.Contracts.Response.ApprovalRes; 
using Puchase_and_payables.Requests;
using System; 
using System.Linq; 
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Approvals
{
    public class GetAllSupplierDataAwaitingApprovalQueryHandler : IRequestHandler<GetAllSupplierDataAwaitingApprovalQuery, SupplierRespObj>
    {
        private readonly ISupplierRepository _repo;
        private readonly IIdentityServerRequest _serverRequest;

        public GetAllSupplierDataAwaitingApprovalQueryHandler(
            ISupplierRepository supplierRepository, 
            IIdentityServerRequest identityServerRequest)
        {
            _repo = supplierRepository;
            _serverRequest = identityServerRequest; 
        }
        public async Task<SupplierRespObj> Handle(GetAllSupplierDataAwaitingApprovalQuery request, CancellationToken cancellationToken)
        { 
            try
            {
                var result = await _serverRequest.GetAnApproverItemsFromIdentityServer();
                if (!result.IsSuccessStatusCode)
                {
                    return new SupplierRespObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage { FriendlyMessage = $"{result.ReasonPhrase} {result.StatusCode}" }
                        }
                    };
                }

                var data = await result.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data);

                if (res == null)
                {
                    return new SupplierRespObj
                    {
                        Status = res.Status
                    };
                }

                if (res.workflowTasks.Count() < 1)
                {
                    return new SupplierRespObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = true,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "No Pending Approval"
                            }
                        }
                    }; 
                }

                var supplierType = await _repo.GetAllSupplierTypeAsync();
                var supplier = await _repo.GetSupplierDataAwaitingApprovalAsync(res.workflowTasks.Select(x => x.TargetId).ToList(), res.workflowTasks.Select(d => d.WorkflowToken).ToList());


                return new SupplierRespObj
                {
                    Suppliers = supplier.Select(d => new SupplierObj {
                        Name = d.Name,
                        SupplierTypeId = d.SupplierTypeId,
                        Active = d.Active,
                        Address = d.Address,
                        ApprovalStatusId = d.ApprovalStatusId,
                        CountryId = d.CountryId,
                        Email = d.Email,
                        ExpiryDate = d.ExpiryDate,
                        CreatedBy = d.CreatedBy,
                        CreatedOn = d.CreatedOn,
                        Deleted = d.Deleted,
                        HaveWorkPrintPermit = d.HaveWorkPrintPermit,
                        Passport = d.Passport,
                        PhoneNo = d.PhoneNo,
                        PostalAddress = d.PostalAddress,
                        RegistrationNo = d.RegistrationNo,
                        SupplierId = d.SupplierId,
                        SupplierNumber = d.SupplierNumber,
                        SupplierTypeName = supplierType.FirstOrDefault(s => s.SupplierTypeId == d.SupplierTypeId)?.SupplierTypeName ?? "",
                        TaxIDorVATID = d.TaxIDorVATID,
                        UpdatedBy = d.UpdatedBy,
                        Website = d.Website,
                        UpdatedOn = d.UpdatedOn,
                        WorkflowToken = d.WorkflowToken,
                        StatusName = Convert.ToString((ApprovalStatus)d.ApprovalStatusId),
                        Particulars = d.Particulars, 
                    }).ToList(),
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = supplier.Count() < 1 ? "No supplier detail awaiting approvals" : null
                        }
                    }
                }; 
                }
                catch (SqlException ex)
                {
                    throw ex;
                } 
        

          
        }
    }
}
