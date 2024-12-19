using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Scaler.Core.Infrastructure;
using Scaler.Core.Services;
using System.Reflection;

namespace Scaler.Server.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ScalerApplicationDbContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ScalerApplicationDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ScalerApplicationDbContext>();
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            builder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"], b => b.MigrationsAssembly(migrationsAssembly));
            builder.UseOpenIddict();

            return new ScalerApplicationDbContext(builder.Options, SystemUserIdAccessor.GetNewAccessor());
        }
    }
}
