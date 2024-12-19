using Microsoft.AspNetCore.Http;
using Scaler.Core.Services.Account;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Scaler.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public class UserIdAccessor(IHttpContextAccessor httpContextAccessor) : IUserIdAccessor
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string? GetCurrentUserId() => _httpContextAccessor.HttpContext?.User.FindFirstValue(Claims.Subject);
    }

    /// <summary>
    /// 
    /// </summary>
    public class SystemUserIdAccessor : IUserIdAccessor
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly string? id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        private SystemUserIdAccessor(string? id) => this.id = id;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string? GetCurrentUserId() => id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SystemUserIdAccessor GetNewAccessor(string? id = "SYSTEM") => new(id);
    }
}
