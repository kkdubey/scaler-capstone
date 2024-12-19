using Microsoft.EntityFrameworkCore;
using Scaler.Core.Infrastructure;
using Scaler.Core.Models.Shop;

namespace Scaler.Core.Services.Shop
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public class CustomerService(ScalerApplicationDbContext dbContext) : ICustomerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<Customer> GetTopActiveCustomers(int count) => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetAllCustomersData() => dbContext.Customers
                .Include(c => c.Orders).ThenInclude(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(c => c.Orders).ThenInclude(o => o.Cashier)
                .AsSingleQuery()
                .OrderBy(c => c.Name)
                .ToList();
    }
}
