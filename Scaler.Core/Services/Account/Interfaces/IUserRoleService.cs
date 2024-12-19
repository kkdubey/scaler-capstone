using Scaler.Core.Models.Account;

namespace Scaler.Core.Services.Account
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserRoleService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(ApplicationRole role, IEnumerable<string>? claims);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<ApplicationRole?> GetRoleByIdAsync(string roleId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<ApplicationRole?> GetRoleByNameAsync(string roleName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<ApplicationRole?> GetRoleLoadRelatedAsync(string roleName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string roleName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<(bool Success, string[] Errors)> TestCanDeleteRoleAsync(string roleId);
    }
}
