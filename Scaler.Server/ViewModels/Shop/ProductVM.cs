namespace Scaler.Server.ViewModels.Shop
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductVm
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal BuyingPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal SellingPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UnitsInStock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDiscontinued { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? ProductCategoryName { get; set; }
    }
}
