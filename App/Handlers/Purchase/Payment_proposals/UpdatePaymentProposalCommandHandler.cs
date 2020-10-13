using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.Response.FinanceServer;
using Puchase_and_payables.Contracts.Response.Payment; 
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Invoice;
using Puchase_and_payables.DomainObjects.Purchase;
using Puchase_and_payables.Repository.Invoice;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System; 
using System.IO;
using System.Linq; 
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{ 
    public class UpdatePaymentProposalCommand : IRequest<PaymentTermsRegRespObj>
    {
        public int PaymentTermId { get; set; }
        public int JobStatus { get; set; }
        public IFormFile PhaseDocument { get; set; }
        public class UpdatePaymentProposalCommandHandler : IRequestHandler<UpdatePaymentProposalCommand, PaymentTermsRegRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly ILoggerService _logger;
            private readonly ISupplierRepository _supplierRepository;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IFinanceServerRequest _financeServer;
            private readonly IHttpContextAccessor _accessor;
            private readonly IWebHostEnvironment _env;
            private readonly IInvoiceService _invoice;
            private readonly DataContext _dataContext;
            public UpdatePaymentProposalCommandHandler(
                IPurchaseService purchaseService,
                ILoggerService loggerService,
                IInvoiceService invoice,
                IIdentityServerRequest serverRequest,
                IFinanceServerRequest financeServer,
                DataContext dataContext,
                IWebHostEnvironment webHostEnvironment,
                IHttpContextAccessor accessor,
                ISupplierRepository supplierRepository
                )
            {
                _dataContext = dataContext;
                _accessor = accessor;
                _supplierRepository = supplierRepository;
                _env = webHostEnvironment;
                _logger = loggerService;
                _financeServer = financeServer;
                _repo = purchaseService;
                _invoice = invoice;
                _serverRequest = serverRequest;
            }
            public async Task<PaymentTermsRegRespObj> Handle(UpdatePaymentProposalCommand request, CancellationToken cancellationToken)
            {
                var response = new PaymentTermsRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    var user = await _serverRequest.UserDataAsync();
                    if (user == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable To Process This User";
                        return response; 
                    }
                    using (var tran = await _dataContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var phase = await _repo.GetSinglePaymenttermAsync(request.PaymentTermId);

                            if (phase != null)
                            {
                                if (phase.Status == (int)JobProgressStatus.Executed_Successfully)
                                {
                                    response.Status.Message.FriendlyMessage = "This Phase has already been Executed succesfully";
                                    return response;
                                }
                                if (phase.PaymentStatus == (int)PaymentStatus.Paid)
                                {
                                    response.Status.Message.FriendlyMessage = "Payment already made for this phase";
                                    return response;
                                }
                                var file = _accessor.HttpContext.Request.Form.Files;


                                if (file.Count() > 0)
                                {

                                    if (file[0].FileName.Split('.').Length > 2)
                                    {
                                        response.Status.Message.FriendlyMessage = "Invalid Character detected in file Name";
                                        return response;
                                    }

                                    var folderName = Path.Combine("Resources", "Images");
                                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                                    var fileName = $"{file[0].FileName}-{DateTime.Now.Day.ToString()}-{DateTime.Now.Millisecond.ToString()}-{DateTime.Now.Year.ToString()}-{DateTime.Now.Year.ToString()}"; //ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"') + date.ToString();
                                    var type = file[0].ContentType; //ContentDispositionHeaderValue.Parse(file.ContentDisposition).DispositionType.Trim('"');

                                    var fullPath = _env.WebRootPath + "/Resources/" + fileName;
                                    var dbPath = _env.WebRootPath + "/Resources/" + fileName;

                                    using (FileStream filestrem = System.IO.File.Create(fullPath))
                                    {
                                        await file[0].CopyToAsync(filestrem);
                                        await filestrem.FlushAsync();
                                    }

                                    phase.CcompletionCertificateFullPath = fullPath;
                                    phase.CcompletionCertificateName = fileName;
                                    phase.CcompletionCertificatePath = dbPath;
                                    phase.CcompletionCertificateType = type;
                                }

                                phase.Status = request.JobStatus;
                                phase.PaymentStatus = (int)PaymentStatus.Not_Paid;
                                phase.EntryDate = DateTime.Now;
                                phase.InvoiceGenerated = true;
                                await _repo.AddUpdatePaymentTermsAsync(phase);
                            }


                            var currentbidProposals = _dataContext.cor_paymentterms.Where(q => q.BidAndTenderId == phase.BidAndTenderId && q.ProposedBy == (int)Proposer.STAFF).ToList();
                            if (currentbidProposals.Count() > 0)
                            {
                                var thisBidLpo = _dataContext.purch_plpo.FirstOrDefault(q => q.PLPOId == phase.LPOId);
                                var completed = currentbidProposals.All(q => q.Status == (int)JobProgressStatus.Executed_Successfully);
                                if (completed)
                                {
                                    thisBidLpo.JobStatus = (int)JobProgressStatus.Executed_Successfully;
                                }
                                else
                                {
                                    thisBidLpo.JobStatus = (int)JobProgressStatus.In_Progress;
                                }

                                var cancelled = currentbidProposals.All(q => q.Status == (int)JobProgressStatus.Cancelled);
                                if (cancelled)
                                {
                                    thisBidLpo.JobStatus = (int)JobProgressStatus.Cancelled;
                                }
                                await _repo.AddUpdateLPOAsync(thisBidLpo);
                            }

                            if (phase.Status == (int)JobProgressStatus.Executed_Successfully)
                            {
                                var thisPhaseLPO = await _repo.GetLPOsAsync(phase.LPOId);
                                if (thisPhaseLPO != null)
                                {
                                    var invoice = BuildInvoiceDomainObject(phase, thisPhaseLPO, user);

                                    var entryRequest1 = _invoice.BuildSupplierFirstEntryRequestObject(phase, invoice);
                                    var phaseEntry = await _financeServer.PassEntryAsync(entryRequest1);
                                    if (!phaseEntry.Status.IsSuccessful)
                                    {
                                        await tran.RollbackAsync();
                                        response.Status.IsSuccessful = true;
                                        response.Status.Message.FriendlyMessage = $"Proposal Entry Error Occurred Process Rolled Back:  <br/>{phaseEntry.Status.Message.FriendlyMessage}";
                                        return response;
                                    } 
                                    invoice.CreditGl = entryRequest1.CreditGL;
                                    invoice.DebitGl = entryRequest1.DebitGL;
                                    thisPhaseLPO.DateCompleted = DateTime.UtcNow;
                                    await _repo.AddUpdateLPOAsync(thisPhaseLPO);
                                    await _invoice.CreateUpdateInvoiceAsync(invoice);
                                }
                            }
                            await tran.CommitAsync();
                            response.Status.IsSuccessful = true;
                            response.Status.Message.FriendlyMessage = "Successful";
                            return response;
                        }
                        catch (Exception ex)
                        {
                            await tran.RollbackAsync();
                            _logger.Error($"Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                            response.Status.IsSuccessful = false;
                            response.Status.Message.FriendlyMessage = "Could not process request";
                            response.Status.Message.TechnicalMessage = ex.Message;
                            return response;
                        }
                        finally { await tran.DisposeAsync(); } 
                    } 
                }
                catch (Exception ex)
                {
                    #region Log error to file 
                    var errorCode = ErrorID.Generate(4);
                    _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                    return new PaymentTermsRegRespObj
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


            private inv_invoice BuildInvoiceDomainObject(cor_paymentterms phase, purch_plpo lpo, UserDataResponseObj user)
            {
                ///var supplierdetails = GetGLFromSupplierTypeAsync(lpo.WinnerSupplierId).Result;
                var invoiceNumber = $"{DateTime.Now.Year - 1234}{DateTime.Now.Month}{DateTime.Now.Day}{lpo.PurchaseReqNoteId}{phase.PaymentTermId}";
                return new inv_invoice
                {
                    Address = lpo.Address,
                    LPOId = lpo.PLPOId,
                    AmountPayable = lpo.AmountPayable,
                    ApprovalStatusId = (int)ApprovalStatus.Pending,
                    CompanyId = lpo.CompanyId,
                    DeliveryDate = lpo.DeliveryDate,
                    Description = lpo.Description,
                    Amount = phase.NetAmount,
                    InvoiceNumber = invoiceNumber,
                    LPONumber = lpo.LPONumber,
                    PaymentTermId = phase.PaymentTermId,
                    RequestDate = lpo.RequestDate,
                    SupplierId = lpo.WinnerSupplierId,
                };
            }

            //private FinancialTransaction BuildEntryRequestObject(cor_paymentterms request, purch_plpo lpo)
            //{
            //    cor_suppliertype suppliertype = new cor_suppliertype();
            //    var supplierDetail = _supplierRepository.GetSupplierAsync(lpo.WinnerSupplierId).Result;
            //    if (supplierDetail != null)
            //    {
            //        suppliertype = _supplierRepository.GetSupplierTypeAsync(supplierDetail.SupplierTypeId).Result;
            //    }
            //    return new FinancialTransaction
            //    {
            //        Amount = request.NetAmount,
            //        ApprovedBy = "staff",
            //        ApprovedDate = DateTime.Now,
            //        CompanyId = request.CompanyId,
            //        CreditGL = suppliertype.CreditGL,
            //        DebitGL = suppliertype.DebitGL,
            //        CurrencyId = 0,
            //        ReferenceNo = $"{DateTime.Now.Year - 1234}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.UtcNow.Minute}{DateTime.UtcNow.Second}{DateTime.UtcNow.Millisecond}{lpo.PurchaseReqNoteId}{request.PaymentTermId}",
            //        Description = "Payment Proposal first Entry",
            //        OperationId = (int)OperationsEnum.PaymentApproval,
            //        TransactionDate = DateTime.Now,
            //    };
            //}

        } 
    }
    
}