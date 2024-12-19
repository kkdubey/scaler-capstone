using Scaler.Core.Models.Account;
using System.Collections.ObjectModel;

namespace Scaler.Core.Services.Account
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApplicationPermissions
    {

        /// <summary>
        /// User Permissions
        /// </summary>
        public const string UsersPermissionGroupName = "User Permissions";

        /// <summary>
        /// 
        /// </summary>
        public static readonly ApplicationPermission ViewUsers = new(
            "View Users",
            "users.view",
            UsersPermissionGroupName,
            "Permission to view other users account details");

        /// <summary>
        /// 
        /// </summary>
        public static readonly ApplicationPermission ManageUsers = new(
            "Manage Users",
            "users.manage",
            UsersPermissionGroupName,
            "Permission to create, delete and modify other users account details");

        /// <summary>
        /// ROLE PERMISSIONS
        /// </summary>
        public const string RolesPermissionGroupName = "Role Permissions";

        /// <summary>
        /// 
        /// </summary>
        public static readonly ApplicationPermission ViewRoles = new(
            "View Roles",
            "roles.view",
            RolesPermissionGroupName,
            "Permission to view available roles");

        /// <summary>
        /// 
        /// </summary>
        public static readonly ApplicationPermission ManageRoles = new(
            "Manage Roles",
            "roles.manage",
            RolesPermissionGroupName,
            "Permission to create, delete and modify roles");

        /// <summary>
        /// 
        /// </summary>
        public static readonly ApplicationPermission AssignRoles = new(
            "Assign Roles",
            "roles.assign",
            RolesPermissionGroupName,
            "Permission to assign roles to users");

        /// <summary>
        /// ALL PERMISSIONS
        /// </summary>
        public static readonly ReadOnlyCollection<ApplicationPermission> AllPermissions =
            new List<ApplicationPermission> {
                ViewUsers, ManageUsers,
                ViewRoles, ManageRoles, AssignRoles
            }.AsReadOnly();

        /// <summary>
        /// HELPER METHODS
        /// </summary>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public static ApplicationPermission? GetPermissionByName(string? permissionName) => AllPermissions.SingleOrDefault(p => p.Name == permissionName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionValue"></param>
        /// <returns></returns>
        public static ApplicationPermission? GetPermissionByValue(string? permissionValue) => AllPermissions.SingleOrDefault(p => p.Value == permissionValue);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllPermissionValues() => AllPermissions.Select(p => p.Value).ToArray();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] GetAdministrativePermissionValues() => [ManageUsers, ManageRoles, AssignRoles];
    }
}
