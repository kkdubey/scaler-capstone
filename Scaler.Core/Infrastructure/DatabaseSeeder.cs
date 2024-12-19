using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scaler.Core.Models;
using Scaler.Core.Models.Account;
using Scaler.Core.Models.Shop;
using Scaler.Core.Services.Account;

namespace Scaler.Core.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="logger"></param>
    /// <param name="userAccountService"></param>
    /// <param name="userRoleService"></param>
    public class DatabaseSeeder(ScalerApplicationDbContext dbContext, ILogger<DatabaseSeeder> logger,
        IUserAccountService userAccountService, IUserRoleService userRoleService) : IDatabaseSeeder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task InitialSeedAsync()
        {
            await dbContext.Database.MigrateAsync();
            await SeedDefaultInitialUsersAsync();
            await SeedCustomerDataAsync();
        }

        /************ DEFAULT USERS **************/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task SeedDefaultInitialUsersAsync()
        {
            if (!await dbContext.Users.AnyAsync())
            {
                logger.LogInformation("Generating inbuilt accounts");

                const string adminRoleName = "administrator";
                const string userRoleName = "user";

                await EnsureInitialRoleAsync(adminRoleName, "Default administrator",
                    ApplicationPermissions.GetAllPermissionValues());

                await EnsureInitialRoleAsync(userRoleName, "Default user", []);

                await CreateInitialUserAsync("admin",
                                      "tempP@ss123",
                                      "Inbuilt Administrator",
                                      "admin@scaler.com",
                                      "+1 (123) 000-0000",
                                      [adminRoleName]);

                await CreateInitialUserAsync("user",
                                      "tempP@ss123",
                                      "Inbuilt Standard User",
                                      "user@scaler.com",
                                      "+1 (123) 000-0001",
                                      [userRoleName]);

                logger.LogInformation("Inbuilt account generation completed");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="description"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        /// <exception cref="UserRoleException"></exception>
        private async Task EnsureInitialRoleAsync(string roleName, string description, string[] claims)
        {
            if (await userRoleService.GetRoleByNameAsync(roleName) == null)
            {
                logger.LogInformation("Generating initial role: {roleName}", roleName);

                var applicationRole = new ApplicationRole(roleName, description);

                var result = await userRoleService.CreateRoleAsync(applicationRole, claims);

                if (!result.Succeeded)
                {
                    throw new UserRoleException($"Seeding \"{description}\" role failed. Errors: " +
                        $"{string.Join(Environment.NewLine, result.Errors)}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="fullName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        /// <exception cref="UserAccountException"></exception>
        private async Task<ApplicationUser> CreateInitialUserAsync(
            string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
        {
            logger.LogInformation("Generating default user: {userName}", userName);

            var applicationUser = new ApplicationUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true
            };

            var result = await userAccountService.CreateUserAsync(applicationUser, roles, password);

            if (!result.Succeeded)
            {
                throw new UserAccountException($"Seeding \"{userName}\" user failed. Errors: " +
                    $"{string.Join(Environment.NewLine, result.Errors)}");
            }

            return applicationUser;
        }

        /************ DEMO DATA **************/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task SeedCustomerDataAsync()
        {
            if (!await dbContext.Customers.AnyAsync() && !await dbContext.ProductCategories.AnyAsync())
            {
                logger.LogInformation("Seeding demo data");

                var cust1 = new Customer
                {
                    Name = "Komal Dube",
                    Email = "contact@scaler.com",
                    Gender = Gender.Male
                };

                var cust2 = new Customer
                {
                    Name = "test cust2",
                    Email = "cust2@scaler.com",
                    PhoneNumber = "+81123456789",
                    Address = "Some fictional Address, Street 123, test",
                    City = "test",
                    Gender = Gender.Male
                };

                var cust3 = new Customer
                {
                    Name = "John Doe",
                    Email = "johndoe@scaler.com",
                    PhoneNumber = "+18585858",
                    Address = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio.
                    Praesent libero. Sed cursus ante dapibus diam. Sed nisi. Nulla quis sem at elementum imperdiet",
                    City = "Lorem Ipsum",
                    Gender = Gender.Male
                };

                var cust4 = new Customer
                {
                    Name = "Jane Doe",
                    Email = "Janedoe@scaler.com",
                    PhoneNumber = "+18585858",
                    Address = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio.
                    Praesent libero. Sed cursus ante dapibus diam. Sed nisi. Nulla quis sem at elementum imperdiet",
                    City = "Lorem Ipsum",
                    Gender = Gender.Male
                };

                var prodCat1 = new ProductCategory
                {
                    Name = "None",
                    Description = "Default category. Products that have not been assigned a category"
                };

                var prod1 = new Product
                {
                    Name = "BMW M6",
                    Description = "Yet another masterpiece from the world's best car manufacturer",
                    BuyingPrice = 109775,
                    SellingPrice = 114234,
                    UnitsInStock = 12,
                    IsActive = true,
                    ProductCategory = prodCat1
                };

                var prod2 = new Product
                {
                    Name = "Nissan Patrol",
                    Description = "A true man's choice",
                    BuyingPrice = 78990,
                    SellingPrice = 86990,
                    UnitsInStock = 4,
                    IsActive = true,
                    ProductCategory = prodCat1
                };

                var ordr1 = new Order
                {
                    Discount = 500,
                    Cashier = await dbContext.Users.OrderBy(u => u.UserName).FirstAsync(),
                    Customer = cust1
                };

                var ordr2 = new Order
                {
                    Cashier = await dbContext.Users.OrderBy(u => u.UserName).FirstAsync(),
                    Customer = cust2
                };

                ordr1.OrderDetails.Add(new()
                {
                    UnitPrice = prod1.SellingPrice,
                    Quantity = 1,
                    Product = prod1,
                    Order = ordr1
                });
                ordr1.OrderDetails.Add(new()
                {
                    UnitPrice = prod2.SellingPrice,
                    Quantity = 1,
                    Product = prod2,
                    Order = ordr1
                });

                ordr2.OrderDetails.Add(new()
                {
                    UnitPrice = prod2.SellingPrice,
                    Quantity = 1,
                    Product = prod2,
                    Order = ordr2
                });

                dbContext.Customers.Add(cust1);
                dbContext.Customers.Add(cust2);
                dbContext.Customers.Add(cust3);
                dbContext.Customers.Add(cust4);

                dbContext.Products.Add(prod1);
                dbContext.Products.Add(prod2);

                dbContext.Orders.Add(ordr1);
                dbContext.Orders.Add(ordr2);

                await dbContext.SaveChangesAsync();

                logger.LogInformation("Seeding demo data completed");
            }
        }
    }
}
