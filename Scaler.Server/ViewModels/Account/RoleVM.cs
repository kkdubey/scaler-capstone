using Scaler.Core.Extensions;
using Scaler.Server.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Scaler.Server.ViewModels.Account
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleVm : ISanitizeModel
    {
        public virtual void SanitizeModel()
        {
            Id = Id.NullIfWhiteSpace();
            Name = Name.NullIfWhiteSpace();
            Description = Description.NullIfWhiteSpace();
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Role name is required"),
         StringLength(200, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 200 characters")]
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UsersCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PermissionVm[]? Permissions { get; set; }
    }
}
