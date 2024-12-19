using Scaler.Core.Models.Account;
using System.Diagnostics.CodeAnalysis;

namespace Scaler.Server.ViewModels.Account
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionVm
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>

        [return: NotNullIfNotNull(nameof(permission))]
        public static explicit operator PermissionVm?(ApplicationPermission? permission)
        {
            if (permission == null)
                return null;

            return new PermissionVm
            {
                Name = permission.Name,
                Value = permission.Value,
                GroupName = permission.GroupName,
                Description = permission.Description
            };
        }
    }
}
