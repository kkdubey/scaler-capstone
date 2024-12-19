using Microsoft.AspNetCore.Identity;
using Scaler.Core.Models.Shop;

namespace Scaler.Core.Models.Account
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string? FriendlyName
        {
            get
            {
                return string.IsNullOrWhiteSpace(FullName) ? UserName : FullName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Configuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLockedOut => LockoutEnabled && LockoutEnd >= DateTimeOffset.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// 
        /// </summary>
        public ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Order> Orders { get; } = new List<Order>();
    }
}
