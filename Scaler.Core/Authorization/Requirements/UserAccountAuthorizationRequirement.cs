using Microsoft.AspNetCore.Authorization;
using Scaler.Core.Services;
using Scaler.Core.Services.Account;
using System.Security.Claims;

namespace Scaler.Core.Authorization.Requirements
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="operationName"></param>
    public class UserAccountAuthorizationRequirement(string operationName) : IAuthorizationRequirement
    {
        public string OperationName { get; private set; } = operationName;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ViewUserHandler : AuthorizationHandler<UserAccountAuthorizationRequirement, string>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, UserAccountAuthorizationRequirement requirement, string targetUserId)
        {
            if (requirement.OperationName != UserAccountManagementOperations.ReadOperationName)
                return Task.CompletedTask;

            if (context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.ViewUsers)
                || IsSameUser(context.User, targetUserId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        private static bool IsSameUser(ClaimsPrincipal user, string targetUserId)
        {
            return !string.IsNullOrWhiteSpace(targetUserId) && Utilities.GetUserId(user) == targetUserId;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ManageUserHandler : AuthorizationHandler<UserAccountAuthorizationRequirement, string>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, UserAccountAuthorizationRequirement requirement, string targetUserId)
        {
            if ((requirement.OperationName != UserAccountManagementOperations.CreateOperationName &&
                 requirement.OperationName != UserAccountManagementOperations.UpdateOperationName &&
                 requirement.OperationName != UserAccountManagementOperations.DeleteOperationName))
                return Task.CompletedTask;

            if (context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.ManageUsers)
                || IsSameUser(context.User, targetUserId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        private static bool IsSameUser(ClaimsPrincipal user, string targetUserId)
        {
            return !string.IsNullOrWhiteSpace(targetUserId) && Utilities.GetUserId(user) == targetUserId;
        }
    }
}