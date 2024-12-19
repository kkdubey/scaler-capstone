using Microsoft.AspNetCore.Authorization;
using Scaler.Core.Services.Account;
using System.Security.Claims;

namespace Scaler.Core.Authorization.Requirements
{
    /// <summary>
    /// 
    /// </summary>
    public class AssignRolesAuthorizationRequirement : IAuthorizationRequirement
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class AssignRolesHandler :
        AuthorizationHandler<AssignRolesAuthorizationRequirement, (string[] newRoles, string[] currentRoles)>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AssignRolesAuthorizationRequirement requirement, (string[] newRoles, string[] currentRoles) roles)
        {
            if (IsRolesChanged(roles.newRoles, roles.currentRoles))
            {
                if (!context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.AssignRoles))
                    return Task.CompletedTask;
                // If user has View Roles permission, then he can assign any roles
                if (context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.ViewRoles))
                    context.Succeed(requirement);

                // Else user can only assign roles they're part of role
                else if (IsUserInAllAddedRoles(context.User, roles.newRoles, roles.currentRoles))
                    context.Succeed(requirement);
            }
            else
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRoles"></param>
        /// <param name="currentRoles"></param>
        /// <returns></returns>
        private static bool IsRolesChanged(string[] newRoles, string[] currentRoles)
        {
            newRoles ??= [];
            currentRoles ??= [];

            var roleAdded = newRoles.Except(currentRoles).Any();
            var roleRemoved = currentRoles.Except(newRoles).Any();

            return roleAdded || roleRemoved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextUser"></param>
        /// <param name="newRoles"></param>
        /// <param name="currentRoles"></param>
        /// <returns></returns>
        private static bool IsUserInAllAddedRoles(ClaimsPrincipal contextUser, string[] newRoles, string[] currentRoles)
        {
            newRoles ??= [];
            currentRoles ??= [];

            var addedRoles = newRoles.Except(currentRoles);

            return addedRoles.All(contextUser.IsInRole);
        }
    }
}