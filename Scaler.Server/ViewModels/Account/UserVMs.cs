using Scaler.Core.Extensions;
using Scaler.Server.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Scaler.Server.ViewModels.Account
{
    /// <summary>
    /// 
    /// </summary>
    public class UserVm : UserBaseVm
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsLockedOut { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MinimumCount(1, ErrorMessage = "Roles cannot be empty")]
        public string[]? Roles { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserEditVm : UserBaseVm
    {
        /// <summary>
        /// 
        /// </summary>
        public string? CurrentPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MinLength(6, ErrorMessage = "New Password must be at least 6 characters")]
        public string? NewPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MinimumCount(1, false, ErrorMessage = "Roles cannot be empty")]
        public string[]? Roles { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserPatchVm
    {
        /// <summary>
        /// 
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Configuration { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class UserBaseVm : ISanitizeModel
    {
        public virtual void SanitizeModel()
        {
            Id = Id.NullIfWhiteSpace();
            FullName = FullName.NullIfWhiteSpace();
            PhoneNumber = PhoneNumber.NullIfWhiteSpace();
            Configuration = Configuration.NullIfWhiteSpace();
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Username is required"),
         StringLength(200, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 200 characters")]
        public required string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Email is required"),
         StringLength(200, ErrorMessage = "Email must be at most 200 characters"),
         EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Configuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
