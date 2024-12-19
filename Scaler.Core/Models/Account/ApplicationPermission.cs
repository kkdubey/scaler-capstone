using System.Diagnostics.CodeAnalysis;

namespace Scaler.Core.Models.Account
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="groupName"></param>
    /// <param name="description"></param>
    public class ApplicationPermission(string name, string value, string groupName, string? description = null)
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; } = value;

        /// <summary>
        /// 
        /// </summary>
        public string GroupName { get; set; } = groupName;

        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; } = description;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>
        [return: NotNullIfNotNull(nameof(permission))]
        public static implicit operator string?(ApplicationPermission? permission)
        {
            return permission?.Value;
        }
    }
}
