using Scaler.Core.Models.Shop;

namespace Scaler.Core.Services.Shop
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<Customer> GetTopActiveCustomers(int count);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Customer> GetAllCustomersData();
    }
}
