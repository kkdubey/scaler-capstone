using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Scaler.Server.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class OidcServerConfiguration
    {
        public const string ScalerClientId = "scaler_spa";
        public const string SwaggerClientId = "swagger_ui";
        public const string ServerName = "Scaler API";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static async Task RegisterAllClientApplicationsAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

            // Swagger UI Client
            if (await manager.FindByClientIdAsync(SwaggerClientId) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = SwaggerClientId,
                    ClientType = ClientTypes.Public,
                    DisplayName = "Swagger UI",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.Password,
                        "ScalerAPI"
                    }
                });
            }

            // SPA Client
            if (await manager.FindByClientIdAsync(ScalerClientId) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = ScalerClientId,
                    ClientType = ClientTypes.Public,
                    DisplayName = "Scaler SPA",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Phone,
                        Permissions.Scopes.Address,
                        Permissions.Scopes.Roles,
                        "ScalerAPI"
                    }
                });
            }

        }
    }
}
