using GOSLibraries.GOS_API_Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.PPEServer
{
    public class AssetClassificationObj
    {
        public int AsetClassificationId { get; set; }
        public string ClassificationName { get; set; }
        public int UsefulLifeMin { get; set; }
        public int UsefulLifeMax { get; set; }
        public decimal ResidualValue { get; set; }
        public bool Depreciable { get; set; }
        public string DepreciationMethod { get; set; }
        public int SubGlAddition { get; set; }
        public string SubGlAdditionName { get; set; }
        public string SubGlAdditionCode { get; set; }
        public int SubGlDepreciation { get; set; }
        public string SubGlDepreciationName { get; set; }
        public string SubGlDepreciationCode { get; set; }
        public int SubGlAccumulatedDepreciation { get; set; }
        public string SubGlAccumulatedDepreciationName { get; set; }
        public string SubGlAccumulatedDepreciationCode { get; set; }
        public int SubGlDisposal { get; set; }
        public string SubGlDisposalName { get; set; }
        public string SubGlDisposalCode { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class AssetClassificationRespObj
    {
        public List<AssetClassificationObj> AssetClassifications { get; set; }
        public byte[] export { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class UpdatePPELPO : IRequest<UpdatePPELPORegResp>
    {
        public int PLPOId { get; set; }
 
        public string Name { get; set; } 
        public string Address { get; set; }
        public string SupplierIds { get; set; } 
        public decimal Tax { get; set; } 
        public decimal Total { get; set; }
         
        public DateTime DeliveryDate { get; set; } 
        public string LPONumber { get; set; }
         
        public string Description { get; set; }

        public int ApprovalStatusId { get; set; }
        //.....
        public string SupplierNumber { get; set; }
        public string SupplierAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal AmountPayable { get; set; }
        public int JobStatus { get; set; }
        public bool BidComplete { get; set; } = false;
        public int BidAndTenderId { get; set; }
        public int WinnerSupplierId { get; set; }
        public string WorkflowToken { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int PurchaseReqNoteId { get; set; }
        public string Taxes { get; set; }
        public int DebitGl { get; set; }
        public int ServiceTerm { get; set; }
        public int AsetClassificationId;
        public int SubGlAddition;
    }

    public class UpdatePPELPORegResp
    {
        public int LpoId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
