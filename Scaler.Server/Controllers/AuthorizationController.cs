using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Scaler.Core.Models.Account;
using Scaler.Core.Services.Account;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Scaler.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizationController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
        : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsPasswordGrantType())
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    return GetForbidResult("Username or password cannot be empty.");

                var user = await userManager.FindByNameAsync(request.Username)
                    ?? await userManager.FindByEmailAsync(request.Username);

                if (user == null)
                    return GetForbidResult("Please check that your username and password is correct.");

                if (!user.IsEnabled)
                    return GetForbidResult("The specified user account is disabled.");

                var result =
                    await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

                if (result.IsLockedOut)
                    return GetForbidResult("The specified user account has been suspended.");

                if (result.IsNotAllowed)
                    return GetForbidResult("The specified user is not allowed to sign in.");

                if (!result.Succeeded)
                    return GetForbidResult("Please check that your username and password is correct.");

                var principal = await CreateClaimsPrincipalAsync(user, request.GetScopes());

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsRefreshTokenGrantType())
            {
                var authenticateResult =
                    await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                var userId = authenticateResult?.Principal?.GetClaim(Claims.Subject);
                var user = userId != null ? await userManager.FindByIdAsync(userId) : null;

                if (user == null)
                    return GetForbidResult("The refresh token is no longer valid.");

                if (!user.IsEnabled)
                    return GetForbidResult("The user account is disabled.");

                if (!await signInManager.CanSignInAsync(user))
                    return GetForbidResult("The user is no longer allowed to sign in. Please contact admin.");

                var scopes = request.GetScopes();
                if (scopes.Length == 0 && authenticateResult?.Principal != null)
                    scopes = authenticateResult.Principal.GetScopes();

                // Recreate the claims principal in case they changed since the refresh token was issued.
                var principal = await CreateClaimsPrincipalAsync(user, scopes);

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException($"The grant type \"{request.GrantType}\" is not supported.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorDescription"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private ForbidResult GetForbidResult(string errorDescription, string error = Errors.InvalidGrant)
        {
            var authenticationPropertiesproperties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = error,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorDescription
            });

            return Forbid(authenticationPropertiesproperties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(ApplicationUser user, IEnumerable<string> scopes)
        {
            var userPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            userPrincipal.SetScopes(scopes);

            var identity = userPrincipal.Identity as ClaimsIdentity
                ?? throw new InvalidOperationException("ClaimsPrincipal's Identity is null.");

            if (user.FullName != null) identity.SetClaim(CustomClaims.FullName, user.FullName);
            if (user.Configuration != null) identity.SetClaim(CustomClaims.Configuration, user.Configuration);

            userPrincipal.SetDestinations(GetDestinations);

            return userPrincipal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userClaim"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static IEnumerable<string> GetDestinations(Claim userClaim)
        {
            if (userClaim.Subject == null)
                throw new InvalidOperationException("Claim's Subject is null.");

            switch (userClaim.Type)
            {
                case Claims.Name:
                    if (userClaim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    if (userClaim.Subject.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case CustomClaims.FullName:
                case CustomClaims.Configuration:
                    if (userClaim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                case CustomClaims.Permission:
                    yield return Destinations.AccessToken;

                    if (userClaim.Subject.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                case "AspNet.Identity.SecurityStamp":
                    yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}
