using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Commands.Supplier; 
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Auth;
using System; 
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, DeleteRespObj>
    { 
        private readonly ILoggerService _logger;
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public DeleteSupplierCommandHandler( 
            DataContext dataContext, 
            ILoggerService loggerService,
            UserManager<ApplicationUser> userManager)
        { 
            _dataContext = dataContext;
            _logger = loggerService;
            _userManager = userManager;
        }
        public async Task<DeleteRespObj> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
            try
            {
                if (request.req.Count() > 0)
                {
                    foreach (var itemId in request.req)
                    {
                        var supplier = await _dataContext.cor_supplier.FindAsync(itemId.TargetId);
                        if(supplier != null)
                        {
                            var identifications =  _dataContext.cor_identification.Where(a => a.SupplierId == supplier.SupplierId).ToList();
                            if(identifications.Count() > 0)
                            {
                                _dataContext.cor_identification.RemoveRange(identifications);
                            }
                            var authorizations = _dataContext.cor_supplierauthorization.Where(a => a.SupplierId == supplier.SupplierId).ToList();
                            if (authorizations.Count() > 0)
                            {
                                _dataContext.cor_supplierauthorization.RemoveRange(authorizations);
                            }
                            var busingessOwner = _dataContext.cor_supplierbusinessowner.Where(a => a.SupplierId == supplier.SupplierId).ToList();
                            if (busingessOwner.Count() > 0)
                            {
                                _dataContext.cor_supplierbusinessowner.RemoveRange(busingessOwner);
                            }
                            var topclient = _dataContext.cor_topclient.Where(a => a.SupplierId == supplier.SupplierId).ToList();
                            if (topclient.Count() > 0)
                            {
                                _dataContext.cor_topclient.RemoveRange(topclient);
                            }
                            var topsuppliers = _dataContext.cor_topsupplier.Where(a => a.SupplierId == supplier.SupplierId).ToList();
                            if (topsuppliers.Count() > 0)
                            {
                                _dataContext.cor_topsupplier.RemoveRange(topsuppliers);
                            }
                            var bankdetails = _dataContext.cor_bankaccountdetail.Where(a => a.SupplierId == supplier.SupplierId).ToList();
                            if (bankdetails.Count() > 0)
                            {
                                _dataContext.cor_bankaccountdetail.RemoveRange(bankdetails);
                            }
                            var financialDetail = _dataContext.cor_financialdetail.Where(a => a.SupplierId == supplier.SupplierId).ToList();
                            if (financialDetail.Count() > 0)
                            {
                                _dataContext.cor_financialdetail.RemoveRange(financialDetail);
                            }
                            var documents = _dataContext.cor_supplierdocument.Where(a => a.SupplierId == supplier.SupplierId).ToList();
                            if (documents.Count() > 0)
                            {
                                _dataContext.cor_supplierdocument.RemoveRange(documents);
                            }
                            var user = await _userManager.FindByEmailAsync(supplier.Email);
                            if(user != null)
                            {
                                await _userManager.DeleteAsync(user);
                            }
                            _dataContext.cor_supplier.Remove(supplier);
                            await _dataContext.SaveChangesAsync();
                        } 
                    }
                }
                response.Deleted = true;
                response.Status.IsSuccessful = true;
                response.Status.Message.FriendlyMessage = "Successful";
                return response; 

            }
            catch (SqlException ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");

                response.Deleted = true;
                response.Status.IsSuccessful = true;
                response.Status.Message.FriendlyMessage = "Error occured!! Unable to delete item";
                response.Status.Message.MessageId = errorCode;
                response.Status.Message.TechnicalMessage = $"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}";
                return response; 
                #endregion
            }
        }
    }
}
