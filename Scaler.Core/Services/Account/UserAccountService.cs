using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scaler.Core.Infrastructure;
using Scaler.Core.Models.Account;

namespace Scaler.Core.Services.Account
{
    /// <inheritdoc />
    public class UserAccountService(ScalerApplicationDbContext context, UserManager<ApplicationUser> userManager)
        : IUserAccountService
    {

        /// <inheritdoc />
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId) => await userManager.FindByIdAsync(userId);

        /// <inheritdoc />
        public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName) => await userManager.FindByNameAsync(userName);

        /// <inheritdoc />
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email) => await userManager.FindByEmailAsync(email);

        /// <inheritdoc />
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user) => await userManager.GetRolesAsync(user);

        /// <inheritdoc />
        public async Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(string userId)
        {
            var user = await context.Users
                .Include(u => u.Roles)
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
                return null;

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();

            var roles = await context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name!)
                .ToArrayAsync();

            return (user, roles);
        }

        /// <inheritdoc />
        public async Task<List<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize)
        {
            IQueryable<ApplicationUser> usersQuery = context.Users
                .Include(u => u.Roles)
                .OrderBy(u => u.UserName);

            if (page != -1)
                usersQuery = usersQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                usersQuery = usersQuery.Take(pageSize);

            var users = await usersQuery.ToListAsync();

            var userRoleIds = users.SelectMany(u => u.Roles.Select(r => r.RoleId)).ToList();

            var roles = await context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();

            return users
                .Select(u => (u, roles.Where(r => u.Roles.Select(ur => ur.RoleId).Contains(r.Id)).Select(r => r.Name!)
                    .ToArray()))
                .ToList();
        }

        /// <inheritdoc />
        public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user,
            IEnumerable<string> roles, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            user = (await userManager.FindByNameAsync(user.UserName!))!;

            try
            {
                result = await userManager.AddToRolesAsync(user, roles.Distinct());
            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            return (true, []);
        }

        /// <inheritdoc />
        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user) => await UpdateUserAsync(user, null);

        /// <inheritdoc />
        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user,
            IEnumerable<string>? roles)
        {
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            if (roles != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var rolesToRemove = userRoles.Except(roles).ToArray();
                var rolesToAdd = roles.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Length != 0)
                {
                    result = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (rolesToAdd.Length != 0)
                {
                    result = await userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return (true, []);
        }

        /// <inheritdoc />
        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user,
            string newPassword)
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);
            return !result.Succeeded ? (false, result.Errors.Select(e => e.Description).ToArray()) : (true, []);
        }

        /// <inheritdoc />
        public async Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user,
            string currentPassword, string newPassword)
        {
            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return !result.Succeeded ? (false, result.Errors.Select(e => e.Description).ToArray()) : (true, []);
        }

        /// <inheritdoc />
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (await userManager.CheckPasswordAsync(user, password)) return true;
            if (!userManager.SupportsUserLockout)
                await userManager.AccessFailedAsync(user);

            return false;

        }

        /// <inheritdoc />
        public async Task<(bool Success, string[] Errors)> TestCanDeleteUserAsync(string userId)
        {
            var errors = new List<string>();

            if (await context.Orders.Where(o => o.CashierId == userId).AnyAsync())
                errors.Add("User has associated orders");

            return (errors.Count == 0, errors.ToArray());
        }

        /// <inheritdoc />
        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            return user != null ? await DeleteUserAsync(user) : (true, []);
        }

        /// <inheritdoc />
        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(ApplicationUser user)
        {
            var result = await userManager.DeleteAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }
    }
}
