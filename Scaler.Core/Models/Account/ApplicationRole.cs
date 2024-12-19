using Microsoft.AspNetCore.Identity;

namespace Scaler.Core.Models.Account
{
    public class ApplicationRole : IdentityRole, IAuditableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationRole()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        public ApplicationRole(string roleName) : base(roleName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="description"></param>
        public ApplicationRole(string roleName, string description) : base(roleName)
        {
            Description = description;
        }

        /// <summary>
        /// Gets or sets the description for this role.
        /// </summary>
        public string? Description { get; set; }

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
        public ICollection<IdentityUserRole<string>> Users { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// 
        /// </summary>
        public ICollection<IdentityRoleClaim<string>> Claims { get; } = new List<IdentityRoleClaim<string>>();
    }
}
