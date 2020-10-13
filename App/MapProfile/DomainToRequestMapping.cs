using AutoMapper;
using GODP.APIsContinuation.DomainObjects.Supplier;
using GODPAPIs.Contracts.Commands.Supplier;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using Puchase_and_payables.Contracts.Response.PPEServer;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.DomainObjects.Purchase;
using Puchase_and_payables.DomainObjects.Supplier;

namespace Puchase_and_payables.MapProfile
{
    public class DomainToRequestMapping : Profile
    {
        public DomainToRequestMapping()
        {
            CreateMap<cor_supplier, SupplierObj>();
            CreateMap<cor_supplierauthorization, SupplierAuthorizationObj>();
            CreateMap<cor_supplierbusinessowner, SupplierBuisnessOwnerObj>();
            CreateMap<cor_supplierdocument, SupplierDocumentObj>();
            CreateMap<cor_topclient, SupplierTopClientObj>();
            CreateMap<cor_topsupplier, SupplierTopSupplierObj>();

            CreateMap<UpdateSupplierAuthorizationCommand, cor_supplierauthorization>();
            CreateMap<cor_supplierbusinessowner, UpdateSupplierBuisnessOwnerCommand>();
            CreateMap<cor_supplier, UpdateSupplierCommand>();
            CreateMap<cor_supplierdocument, UpdateSupplierDocumentCommand>();
            CreateMap<cor_topclient, UpdateSupplierTopClientCommand>();
            CreateMap<cor_topsupplier, UpdateSupplierTopSupplierCommand>();

            CreateMap<cor_financialdetail, SupplierFinancialDetalObj>();
            CreateMap<cor_identification, IdentificationObj>();
            CreateMap<purch_plpo, UpdatePPELPO>();
        }
    }
}
