namespace Scaler.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        string? CreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string? UpdatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        DateTime CreatedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        DateTime UpdatedDate { get; set; }
    }
}
