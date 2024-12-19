using System.ComponentModel.DataAnnotations;

namespace Scaler.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEntity : IAuditableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(40)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(40)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
