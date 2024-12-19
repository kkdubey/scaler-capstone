using Microsoft.AspNetCore.Authorization;
using Scaler.Core.Services.Account;

namespace Scaler.Core.Authorization.Requirements
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewRoleAuthorizationRequirement : IAuthorizationRequirement
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class ViewRoleHandler : AuthorizationHandler<ViewRoleAuthorizationRequirement, string>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, ViewRoleAuthorizationRequirement requirement, string roleName)
        {
            if (context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.ViewRoles)
                || context.User.IsInRole(roleName))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
