using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Scaler.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="logPath"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void QuickLog(string text, string logPath)
        {
            var dirPath = Path.GetDirectoryName(logPath);

            if (string.IsNullOrWhiteSpace(dirPath))
                throw new ArgumentException($"Specified path \"{logPath}\" is invalid", nameof(logPath));

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using var writer = File.AppendText(logPath);
            writer.WriteLine($"{DateTime.Now} - {text}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string? GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirstValue(Claims.Subject)?.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string[] GetRoles(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == Claims.Role)
                .Select(c => c.Value)
                .ToArray();
        }
    }
}
