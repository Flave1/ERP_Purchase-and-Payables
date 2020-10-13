using GOSLibraries;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Requirement
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ERPActivityAttribute : Attribute, IAsyncActionFilter
    {
        public int Activity { get; set; }
        public UserActions Action { get; set; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var response = new MiddlewareResponse { Status = new APIResponseStatus { Message = new APIResponseMessage()  } };
            var userId = context.HttpContext.User?.FindFirst("userId")?.Value ?? string.Empty;
            IEnumerable<string> thisUserRoleIds = new List<string>();
            IEnumerable<string> thisUserRoleNames = new List<string>();
            IEnumerable<string> roleActivities = new List<string>();
            var thisUserRoleCan = false;

            if (string.IsNullOrEmpty(userId))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult(response);
                return;
            }

            using (var scope = context.HttpContext.RequestServices.CreateScope())
            {
                try
                {
                    var scopedServices = scope.ServiceProvider;
                    var serverRequest = scopedServices.GetRequiredService<IIdentityServerRequest>();


                    var userroles = await serverRequest.GetUserRolesAsync();
                    var activities = await serverRequest.GetAllActivityAsync();

                    thisUserRoleIds = userroles.UserRoles.Where(x => x.UserId == userId).ToList().Select(x => x.RoleId);


                    thisUserRoleNames = (from userRole in userroles.UserRoles select userRole.RoleName).ToList();

                    roleActivities = (from activity in activities.Activities join userActivityRole 
                                      in userroles.UserRoleActivities on activity.ActivityId equals userActivityRole.ActivityId
                                      select userActivityRole.RoleName).ToList();
                     

                    bool hasMatch = roleActivities.Select(x => x).Intersect(thisUserRoleNames).Any();

                    if (hasMatch)
                    {
                        if (Action == UserActions.Add)
                            thisUserRoleCan = userroles.UserRoleActivities.Any(x => thisUserRoleIds.Contains(x.RoleId) && x.ActivityId == Activity && x.CanAdd == true);
                        if (Action == UserActions.Approve)
                            thisUserRoleCan = userroles.UserRoleActivities.Any(x => thisUserRoleIds.Contains(x.RoleId) && x.ActivityId == Activity && x.CanApprove == true);
                        if (Action == UserActions.Delete)
                            thisUserRoleCan = userroles.UserRoleActivities.Any(x => thisUserRoleIds.Contains(x.RoleId) && x.ActivityId == Activity && x.CanDelete == true);
                        if (Action == UserActions.Update)
                            thisUserRoleCan = userroles.UserRoleActivities.Any(x => thisUserRoleIds.Contains(x.RoleId) && x.ActivityId == Activity && x.CanEdit == true);
                        if (Action == UserActions.View)
                            thisUserRoleCan = userroles.UserRoleActivities.Any(x => thisUserRoleIds.Contains(x.RoleId) && x.ActivityId == Activity && x.CanView == true);
                    }
                     
                    if (!thisUserRoleNames.Contains(StaticRoles.GODP))
                    {
                        if (!thisUserRoleCan)
                        {
                            var contentResponse = new MiddlewareResponse
                            {
                                Status = new APIResponseStatus
                                {
                                    IsSuccessful = false,
                                    Message = new APIResponseMessage { FriendlyMessage = GenericMiddlwareMessages.NO_PRIVILEGE },
                                }
                            };
                            context.HttpContext.Response.StatusCode = 403;
                            context.Result = new BadRequestObjectResult(contentResponse);
                            return;
                        }
                    }
                    await next();

                }
                catch (Exception ex)
                {
                    var contentResponse = new MiddlewareResponse
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage { FriendlyMessage = ex?.Message, TechnicalMessage = ex.InnerException?.Message }
                        }
                    };
                    context.HttpContext.Response.StatusCode = 500;
                    context.Result = new BadRequestObjectResult(contentResponse);
                    return;
                }
            }
        }


    }
}
