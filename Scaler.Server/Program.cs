using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using Scaler.Core.Authorization;
using Scaler.Core.Authorization.Requirements;
using Scaler.Core.Infrastructure;
using Scaler.Core.Models.Account;
using Scaler.Core.Services;
using Scaler.Core.Services.Account;
using Scaler.Core.Services.Shop;
using Scaler.Server.Configuration;
using Scaler.Server.Services.Email;
using Scaler.ServiceBusMessaging;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Scaler.Server;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var appBuilder = WebApplication.CreateBuilder(args);

        appBuilder.AddServiceDefaults();

        /************* ADD SERVICES *************/

        var dbConnectionString = appBuilder.Configuration.GetConnectionString("DefaultConnection") ??
                                 throw new InvalidOperationException("Connection string 'DefaultConnection' not found. check Application setting json file");

        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

        appBuilder.Services.AddDbContext<ScalerApplicationDbContext>(options =>
        {
            options.UseSqlServer(dbConnectionString, b => b.MigrationsAssembly(migrationsAssembly));
            options.UseOpenIddict();
        });

        // Add Identity
        appBuilder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ScalerApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Configure Identity options and password complexity
        appBuilder.Services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.RoleClaimType = Claims.Role;
            options.ClaimsIdentity.EmailClaimType = Claims.Email;
            options.ClaimsIdentity.UserNameClaimType = Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
            options.User.RequireUniqueEmail = true;
        });

        // Configure OpenIddict
        appBuilder.Services.AddQuartz(options =>
        {
            options.UseInMemoryStore();
            options.UseSimpleTypeLoader();
        });

        // Register the Quartz.NET service
        appBuilder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        appBuilder.Services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseQuartz();
                options.UseEntityFrameworkCore()
                    .UseDbContext<ScalerApplicationDbContext>();
            })
            .AddServer(options =>
            {
                options.SetTokenEndpointUris("connect/token");

                options.AllowPasswordFlow()
                    .AllowRefreshTokenFlow();

                options.RegisterScopes(
                    Scopes.Profile,
                    Scopes.Email,
                    Scopes.Address,
                    Scopes.Phone,
                    Scopes.Roles,
                    "ScalerAPI"
                );

                if (appBuilder.Environment.IsDevelopment())
                {
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();
                }
                else
                {
                    var oidcCertFileName = appBuilder.Configuration["Certificates:OIDC:Path"];
                    var oidcCertFilePassword = appBuilder.Configuration["Certificates:OIDC:Password"];

                    if (string.IsNullOrWhiteSpace(oidcCertFileName))
                    {
                        options.AddEphemeralEncryptionKey()
                            .AddEphemeralSigningKey();
                    }
                    else
                    {
                        var oidcCertificate = new X509Certificate2(oidcCertFileName, oidcCertFilePassword);
                        options.AddEncryptionCertificate(oidcCertificate)
                            .AddSigningCertificate(oidcCertificate);
                    }
                }
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        appBuilder.Services.AddAuthentication(o =>
        {
            o.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            o.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        appBuilder.Services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.ViewAllUsersPolicy,
                policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ViewUsers))
            .AddPolicy(AuthPolicies.ManageAllUsersPolicy,
                policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ManageUsers))
            .AddPolicy(AuthPolicies.ViewAllRolesPolicy,
                policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ViewRoles))
            .AddPolicy(AuthPolicies.ViewRoleByRoleNamePolicy,
                policy => policy.Requirements.Add(new ViewRoleAuthorizationRequirement()))
            .AddPolicy(AuthPolicies.ManageAllRolesPolicy,
                policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ManageRoles))
            .AddPolicy(AuthPolicies.AssignAllowedRolesPolicy,
                policy => policy.Requirements.Add(new AssignRolesAuthorizationRequirement()));

        // Add cors
        appBuilder.Services.AddCors();

        appBuilder.Services.AddControllers();

        appBuilder.Services.AddEndpointsApiExplorer();

        appBuilder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = OidcServerConfiguration.ServerName, Version = "v1" });
            c.OperationFilter<SwaggerAuthorizeCheckFilter>();
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/connect/token", UriKind.Relative)
                    }
                }
            });
        });

        appBuilder.Services.AddAutoMapper(typeof(Program));

        // Configurations
        appBuilder.Services.Configure<AppSettings>(appBuilder.Configuration);

        // Business Services
        appBuilder.Services.AddScoped<ICustomerService, CustomerService>();
        appBuilder.Services.AddScoped<IProductService, ProductService>();
        appBuilder.Services.AddScoped<IOrdersService, OrdersService>();
        appBuilder.Services.AddScoped<IUserAccountService, UserAccountService>();
        appBuilder.Services.AddScoped<IUserRoleService, UserRoleService>();
        appBuilder.Services.AddScoped<ServiceBusSender>();
        appBuilder.Services.AddScoped<ServiceBusTopicSender>();


        // Auth Handlers
        appBuilder.Services.AddSingleton<IAuthorizationHandler, ViewRoleHandler>();
        appBuilder.Services.AddSingleton<IAuthorizationHandler, AssignRolesHandler>();
        appBuilder.Services.AddSingleton<IAuthorizationHandler, ViewUserHandler>();
        appBuilder.Services.AddSingleton<IAuthorizationHandler, ManageUserHandler>();

        // Other Services
        //builder.Services.AddHttpContextAccessor(); // Todo: Test if needed. If IUserIdAccessor works
        appBuilder.Services.AddScoped<IEmailSender, EmailSender>();
        appBuilder.Services.AddScoped<IUserIdAccessor, UserIdAccessor>();

        // DB Creation and Seeding
        appBuilder.Services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();

        //File Logger
        appBuilder.Logging.AddFile(appBuilder.Configuration.GetSection("Logging"));


        var app = appBuilder.Build();

        app.MapDefaultEndpoints();

        /************* CONFIGURE REQUEST PIPELINE *************/

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Swagger UI - Scaler";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{OidcServerConfiguration.ServerName} Version V1");
                c.OAuthClientId(OidcServerConfiguration.SwaggerClientId);
            });

            IdentityModelEventSource.ShowPII = true;
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        /************* SEED DATABASE For first execution *************/

        using var scope = app.Services.CreateScope();
        try
        {
            var dbSeeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
            await dbSeeder.InitialSeedAsync();

            await OidcServerConfiguration.RegisterAllClientApplicationsAsync(scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogCritical(ex, "An error occured whilst creating/seeding database");

            throw;
        }

        /************* RUN APPLICATION *************/

        await app.RunAsync();
    }
}