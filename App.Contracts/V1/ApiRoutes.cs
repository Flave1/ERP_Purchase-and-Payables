using System;
using System.Collections.Generic;

namespace Puchase_and_payables.Contracts.V1
{
    public class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;



        public static class Identity
        {
            public const string IDENTITYSERVERLOGIN = "/identity/login";
            public const string LOGIN = Base + "/ppidentity/login";
            public const string FETCH_USERDETAILS = Base + "/ppidentity/profile";
            public const string IDENTITY_SERVER_FETCH_USERDETAILS = "/identity/profile";
            public const string REGISTER = Base + "/ppidentity/register";
            public const string REFRESHTOKEN = Base + "/ppidentity/refeshToken";
            public const string CHANGE_PASSWORD = Base + "/ppidentity/changePassword";
            public const string CONFIRM_EMAIL = Base + "/ppidentity/confrimEmail";
            public const string CONFIRM_CODE = Base + "/ppidentity/confirmCode";
        }


        public static class CreditServerEndpoint
        {
            public const string SUB_GLS= "/subgl/get/all";
            //CALLED THROUGH CODE 
        }

        public static class PPEServerEndpoint
        {
            public const string UPDATE_LPO = "/addition/update/lponumber";
            public const string GET_ALL_ADDITIONS = "/assetclassification/get/all/assetclassification";
            //CALLED THROUGH CODE 
        }


        public static class FinanceServerRequest
        {
            public const string CREATE_INVOICE = "/payment/create/update/invoice";
            public const string GET_SINGLE_INVOICE = "/payment/get/single/invoice/details";
            public const string PASS_ENTRY = "/financialtransaction/pass/to/entry";
            public const string GET_BANKS = "/bankgl/get/all/bankgl";
            public const string VERIFY_ACCOUNT_NUMBER = "/payment/flutterwave/validate/account/number";
            public const string MAKE_TRANSFER = "/payment/flutterwave/create/transfer";
            public const string GET_ALL_FINANCIAL_ENTRY = "/financialtransaction/get/all/FT";
        }

        public static class IdentitySeverWorkflow
        {
            //CALLED THROUGH CODE
            public const string GO_FOR_APPROVAL = "/workflow/goThroughApprovalFromCode";
            public const string GET_ALL_STAFF_AWAITING_APPROVALS = "/workflow/get/all/staffAwaitingApprovalsFromCode";
            public const string STAFF_APPROVAL_REQUEST = "/workflow/staff/approvaltask";
            public const string GET_ALL_STAFF = "/admin/get/all/staff";
        }

        public static class IdentitySeverRequests
        {
            //CALLED THROUGH CODE
            public const string COMPANY = "/company/get/all/companystructures";
            public const string SEND_EMAIL = "/email/send/emails";
            public const string SEND_EMAIL_TO_SPECIFIC_OFFICERS = "/email/send/specific/emails";
            public const string DOCUMENT_TYPE = "/common/documentypes";
            public const string ACTIVITES = "/admin/get/all/activities";
            public const string COUNTRY = "/common/countries";
            public const string SINGLE_CURRENCY = "/common/get/single/currencyById";
            public const string GET_THIS_ROLES = "/admin/get/this/userroles";
        }


        public static class SupplierEndpoints
        {
            public const string ADD_UPDATE_TERMS = Base + "/supplier/add/update/serviceTerm";
            public const string ADD_UPDATE_SUPPLIER_TYPE = Base + "/supplier/add/update/supplierType";
            public const string ADD_UPDATE_TASK_SETUP = Base + "/supplier/add/update/taxSetup";
            public const string GET_ALL_TERMS = Base + "/supplier/get/all/serviceTerm";
            public const string GET_ALL_SUPPLIER_TYPE = Base + "/supplier/get/all/supplierType";
            public const string GET_ALL_TASK_SETUP = Base + "/supplier/get/all/taxSetup";
            public const string GET_TERMS = Base + "/supplier/get/single/serviceTerm/serviceTermId";
            public const string GET_SUPPLIER_TYPE = Base + "/supplier/single/all/supplierType/supplierTypeId";
            public const string GET_TASK_SETUP = Base + "/supplier/get/single/taxSetup/TaxSetupId";

            public const string UPDATE_SUPPLIER = Base + "/supplier/add/update/supplier";
            public const string GET_ALL_SUPPLIERS = Base + "/supplier/get/all/supplers";
            public const string GET_ALL_PENDING_SUPPLIERS = Base + "/supplier/get/all/pendingsupplers";
            public const string GET_SUPPLIER = Base + "/supplier/get/single/supplerId";
            public const string DELETE_SUPPLIER = Base + "/supplier/delete/supplier/targetIds";

            public const string UPDATE_SUPPLIER_AUTHORIZATION = Base + "/supplier/add/update/authorization";
            public const string GET_ALL_SUPPLIER_AUTHORIZATIONS = Base + "/supplier/get/all/authorizations/supplierId";
            public const string GET_SUPPLIER_AUTHORIZATION = Base + "/supplier/get/single/authorization/authorizationId";
            public const string DELETE_SUPPLIER_AUTHORIZATION = Base + "/supplier/delete/authorization/targetIds";

            public const string UPDATE_SUPPLIER_BUSINESS_OWNER = Base + "/supplier/add/update/businessOwner";
            public const string GET_ALL_SUPPLIER_BUSINESS_OWNER = Base + "/supplier/get/all/businessOwners/supplierId";
            public const string GET_SUPPLIER_BUSINESS_OWNER = Base + "/supplier/get/single/businessOwnerId";
            public const string DELETE_SUPPLIER_BUSINESS_OWNER = Base + "/supplier/delete/businessOwner/targetIds";

            public const string UPDATE_SUPPLIER_DOCUMENT = Base + "/supplier/add/update/document";
            public const string GET_ALL_SUPPLIER_DOCUMENTS = Base + "/supplier/get/all/documents/supplierId";
            public const string GET_SUPPLIER_DOCUMENT = Base + "/supplier/get/single/documentId";
            public const string DELETE_SUPPLIER_DOCUMENT = Base + "/supplier/delete/document/targetIds";

            public const string UPDATE_SUPPLIER_TOP_SUPPLIER = Base + "/supplier/update/topSupplier";
            public const string GET_ALL_TOP_SUPPLIERS = Base + "/supplier/get/all/topSuppliers/supplierId";
            public const string GET_SUPPLIER_TOP_SUPPLIER = Base + "/supplier/get/single/topSupplierId";
            public const string DELETE_SUPPLIER_TOP_SUPPLIER = Base + "/supplier/delete/topSupplier/targetIds";

            public const string UPDATE_SUPPLIER_TOP_CLIENT = Base + "/supplier/update/topClient";
            public const string GET_ALL_TOP_CLIENTS = Base + "/supplier/get/all/topClients/supplierId";
            public const string GET_SUPPLIER_TOP_CLIENT = Base + "/supplier/get/single/topClientId";
            public const string DELETE_SUPPLIER_TOP_CLIENT = Base + "/supplier/delete/topClient/targetIds";

            public const string GET_AWAITING_APPROVALS = Base + "/supplier/gel/all/awaitingAprovals";
            public const string APPROVAL_REQUEST = Base + "/supplier/approval/currentStaffApprovalRequest";

            public const string APPROVAL_DETAILS = Base + "/supplier/get/currentTarget/approvaldetails";

            public const string ADD_UPPDATE_SUPPLIER_BANK_DETA = Base + "/supplier/add/update/bankdetails";
            public const string GET_SUPPLIER_BANK_DETA = Base + "/supplier/get/single/bankdetails/bankdetailId";
            public const string GET_ALL_SUPPLIER_BANK_DETA = Base + "/supplier/get/all/bankdetails/supplierId";

            public const string ADD_UPPDATE_SUPPLIER_FINANCIAL_DETA = Base + "/supplier/add/update/financialdetails";
            public const string GET_SUPPLIER_FINANCIAL_DETA = Base + "/supplier/get/single/bankdetails/financialdetailId";
            public const string GET_ALL_SUPPLIER_FINANCIAL_DETA = Base + "/supplier/get/all/financialDetails/supplierId";
            public const string GO_THROUGH_APPROVAL = Base + "/supplier/go/through/approval";
            public const string GET_LIST_OF_APPROVAED_SUPPLIERS= Base + "/supplier/get/all/approved/suppliers";
            public const string ADD_UPDATE_SUPP_IDENTIFICATION = Base + "/supplier/add/update/identification";
            public const string GET_ALL_SUP_IDENTIFICATION = Base + "/supplier/get/all/identification/supplierId";
            public const string GET_SUP_IDENTIFICATION = Base + "/supplier/get/single/identification/identityId";

            public const string GENERATE_TAXSETUP_FILE = Base + "/supplier/generate/excel/taxsetup";
            public const string UPLOAD_TAXSETUP_FILE = Base + "/supplier/upload/excel/taxsetup";
            public const string GENERATE_SERVICETERM_FILE = Base + "/supplier/generate/excel/serviceterm";
            public const string UPLOAD_SERVICETERM_FILE = Base + "/supplier/upload/excel/serviceterm";
            public const string GENERATE_SUPPLIERTYP_FILE = Base + "/supplier/generate/excel/suppliertype";
            public const string UPLOAD_SUPPLIERTYP_FILE = Base + "/supplier/upload/excel/suppliertype";
            public const string GENERATE_SUPPLIERINFORMATION_FILE = Base + "/supplier/generate/excel/supplierinformation";
            public const string UPLOAD_SUPPLIERINFORMATION_FILE = Base + "/supplier/upload/excel/supplierinformation";

            public const string DELETE_SUPPLIER_TYPE = Base + "/supplier/delete/suppliertype/targetIds";
            public const string DELETE_SUPPLIER_BANK_DETAILS = Base + "/supplier/delete/bankdetails/targetIds";
            public const string DELETE_SUPPLIER_FINANCIAL_DETAILS = Base + "/supplier/delete/financialdetails/targetIds";
            public const string DELETE_SUPPLIER_IDENTIFICATION = Base + "/supplier/delete/identifications/targetIds";
            public const string DELETE_SUPPLIER_SERVICE_TERMS = Base + "/supplier/delete/serviceterms/targetIds";
            public const string DELETE_SUPPLIER_TAX_SETUP = Base + "/supplier/delete/taxsetup/targetIds";
            public const string SUPPLIER_MULTIPLE_APPROVE = Base + "/supplier/multiple/approve";

        }

        public static class OnWebSupplierEndpoints
        {
            public const string GET_THIS_SUPPLIER_DATA= Base + "/onwebsupplier/get/current/supplierInformation";
            public const string GET_SUPPLIER_BIDs = Base + "/onwebsupplier/get/current/supplierbids";
            public const string GET_SUPPLIER_LPO = Base + "/onwebsupplier/get/current/supplierlpos";
            public const string GET_ALL_COUNTRY = Base + "/onwebsupplier/get/all/countries";
            public const string GET_ALL_DOCUMENTYPE = Base + "/onwebsupplier/get/documents";
            public const string GET_BANKS = Base + "/onwebsupplier/get/all/banks";
        }
        public static class Purchases
        {
            public const string ADD_PURCHASE_REQ_NOTE = Base + "/purchase/add/update/purchaseRequisitionNote";
            public const string STAFF_PURCHASE_REQ_NOTE_APPROVAL = Base + "/purchase/staff/approval/purchaseRequisitionNote";
            public const string GET_PRN_AWAITING_APPROVALS = Base + "/purchase/get/all/prn/awaitingAprovals";
            public const string GET_BID_AWAITING_APPROVALS = Base + "/purchase/get/all/bidandtender/awaitingAprovals";
            public const string GET_SINGLE_PRN_DETAIL_APPROVALS = Base + "/purchase/get/single/prnDetails";
            public const string GET_ALL_LPOS = Base + "/purchase/get/all/lpos";
            public const string GET_SUPPLIERS_BY_PRN = Base + "/purchase/get/all/suppliers/byPrndId";
            public const string GET_ALL_BID_AND_TENDERS = Base + "/purchase/get/all/supplierBidandtenders";
            public const string GET_BID_AND_TENDER = Base + "/purchase/get/single/supplierBidandtender";
            public const string GET_ALL_BID_AND_TENDER = Base + "/purchase/get/all/bidandtender";
            public const string ADD_UPDATE_BID_AND_TENDERS = Base + "/purchase/add/update/supplierBidandtenders";
            public const string STAFF_APPROVAL_BID_AND_TENDER = Base + "/purchase/staff/approval/supplierBidandtenders";
            public const string SEND_BID_TO_APPROVAL = Base + "/purchase/send/supplierBid/toApprovals";

            public const string LPO_STAFF_APPRPVAL = Base + "/purchase/staff/approval/lpo";
            public const string UPDATE_PAYMENT = Base + "/purchase/update/paymentTerms";
            public const string GET_ALL_INVOICE = Base + "/purchase/get/all/invoice";
            public const string GET_INVOICE_DETAIL = Base + "/purchase/get/single/invoiceDetail";

            public const string GET_LPO_AWAITING_APPROAL = Base + "/purchase/staff/lpo/awaitingApproval";

            public const string GET_ALL_PRNs = Base + "/purchase/get/all/prns";
            public const string SEND_PRN_FOR_APPROVALS = Base + "/purchase/send/prn/toApprovals";

            public const string GET_ALL_PAYMENTTERMS = Base + "/purchase/get/all/paymentterms";
            public const string SAVE_UPDATE_PAYMENTTERMS = Base + "/purchase/save/update/paymentterms";
            public const string UPDATE_PAYMENTTERMS = Base + "/purchase/update/paymentterms";
            public const string REQUEST_PAYMENT = Base + "/purchase/request/payment";
            public const string GET_SINGLE_LPO = Base + "/purchase/get/single/lpo";
            public const string SEND_LPO_TO_APPROVALS = Base + "/purchase/send/lpo/toapproval";
            public const string UPDATE_LPO_TO_APPROVALS = Base + "/purchase/update/lpo/";
            public const string STAFF_REQUEST_PAYMENT_APPROVAL = Base + "/purchase/staff/request/paymentapproval";
            public const string GET_ALL_REQUESTED_PAYEMENT_AWAITINNG_APPROVAL = Base + "/purchase/get/requestedpayments/awaitingAapprovals";

            public const string GET_SINGLE_BIDAND_TENDER = Base + "/purchase/get/single/bidandtender";
            public const string DELETE_PROPOSAL = Base + "/purchase/delete/payment/proposal";
            public const string SAVE_PROPOSAL_PHASE = Base + "/purchase/update/proposal/phase";
            public const string SEND_PROPOSAL_PHASE_TO_APPROVAL = Base + "/purchase/send/proposal/phase/toapproval";
            public const string RESPOND_TO_LPO = Base + "/purchase/respond/tolpo";
            public const string SUPPLIER_UPDATE_PROPOSAL = Base + "/purchase/update/phase/bysupplier";
            public const string update_ppe_lpo = Base + "/purchase/updateppe/lpo";
            public const string STAFF_PAYMENT = Base + "/purchase/staff/payment/approval";
            public const string PAYMENT_WAITING_APPROVALS = Base + "/purchase/staff/payment/awaitingapprovals";
            public const string GET_ALL_APPROVED_PRNS = Base + "/purchase/get/all/approved/prns";
            public const string UPLOAD_PROPOSAL = Base + "/purchase/upload/proposal";
            public const string DOWNLOAD_PROPOSAL = Base + "/purchase/download/supplier/proposal";
            public const string GET_NOT_BIDDEN_ADVERTS = Base + "/purchase/get/all/notbidden/adverts";
            public const string DOWNLOAD_COMPLETION_CERT = Base + "/purchase/download/certificate";
            public const string BID_BY_STAFF = Base + "/purchase/avertbid/bystaff";

        }
        public static class Report
        {
            public const string AGINGANALYSIS_TABLE = Base + "/report/get/aginganalysis/table/report";
            public const string LPO_REPORT = Base + "/report/get/lpo/report";
            public const string DOWNLOAD_AGINGANALYSIS_TABLE = Base + "/report/download/aginganalysis/report";
            public const string DOWNLOAD_PRN_REPORT = Base + "/report/download/prn/report";
            public const string AGINGANALYSIS_CHART = Base + "/report/get/aginganalysis/chart";
            public const string DASHBOARD_COUNTS = Base + "/report/get/dashboard/Counts";
            public const string PAYABLE_DAYS = Base + "/report/get/payabledays/Counts";
            public const string DASHBOARD_AGE_ANALYSIS = Base + "/report/get/aginganalysis";
            public const string PROJECT_STATUS = Base + "/report/get/projectstatus";

            public const string PAYABLEDAYS_ANALYSIS = Base + "/report/get/payabledays/trendanalysis";
            public const string PURCHASES = Base + "/report/get/purchases/trendanalysis";
            public const string PURCH_AND_PAYABLES_REPORT = Base + "/report/purchases/and/payables/report";
        }
    }
}
