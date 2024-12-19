using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Scaler.Core.Authorization;
using Scaler.Core.Models.Account;
using Scaler.Core.Services.Account;
using Scaler.Server.ViewModels.Account;

namespace Scaler.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/account/users")]
    [Authorize]
    public class UserAccountController(
        ILogger<UserAccountController> logger,
        IMapper mapper,
        IUserAccountService userAccountService,
        IAuthorizationService authorizationService)
        : BaseApiController(logger, mapper)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("")]
        [Authorize(AuthPolicies.ManageAllUsersPolicy)]
        [ProducesResponseType(201, Type = typeof(UserVm))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Register([FromBody] UserEditVm user)
        {
            if (!(await authorizationService.AuthorizeAsync(User, (user.Roles, Array.Empty<string>()),
                AuthPolicies.AssignAllowedRolesPolicy)).Succeeded)
                return new ChallengeResult();

            if (string.IsNullOrWhiteSpace(user.NewPassword))
                AddModelError($"{nameof(user.NewPassword)} is required when registering a new user",
                    nameof(user.NewPassword));

            if (user.Roles == null)
                AddModelError($"{nameof(user.Roles)} is required when registering a new user", nameof(user.Roles));

            if (ModelState.IsValid)
            {
                var appUser = _mapper.Map<ApplicationUser>(user);
                var result = await userAccountService.CreateUserAsync(appUser, user.Roles!, user.NewPassword!);

                if (result.Succeeded)
                {
                    var userVm = await GetUserViewModelHelper(appUser.Id);
                    return CreatedAtAction(nameof(GetUserById), new { id = userVm?.Id }, userVm);
                }

                AddModelError(result.Errors);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserAccountException"></exception>
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(UserVm))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!(await authorizationService.AuthorizeAsync(User, id,
                UserAccountManagementOperations.DeleteOperationRequirement)).Succeeded)
                return new ChallengeResult();

            var appUser = await userAccountService.GetUserByIdAsync(id);

            if (appUser == null)
                return NotFound(id);

            var canDelete = await userAccountService.TestCanDeleteUserAsync(id);
            if (!canDelete.Success)
            {
                AddModelError($"User \"{appUser.UserName}\" cannot be deleted at this time. " +
                    "Delete the associated records and try again");
                AddModelError(canDelete.Errors, "Records");
            }

            if (ModelState.IsValid)
            {
                var userVm = await GetUserViewModelHelper(appUser.Id);
                var result = await userAccountService.DeleteUserAsync(appUser);

                if (!result.Succeeded)
                {
                    throw new UserAccountException($"The following errors occurred whilst deleting user \"{id}\": " +
                        $"{string.Join(", ", result.Errors)}");
                }

                return Ok(userVm);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserAccountException"></exception>
        [HttpPut("unblock/{id}")]
        [Authorize(AuthPolicies.ManageAllUsersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UnblockUser(string id)
        {
            var appUser = await userAccountService.GetUserByIdAsync(id);

            if (appUser == null)
                return NotFound(id);

            appUser.LockoutEnd = null;
            var result = await userAccountService.UpdateUserAsync(appUser);

            if (!result.Succeeded)
            {
                throw new UserAccountException($"Errors occurred whilst unblocking user: " +
                    $"{string.Join(", ", result.Errors)}");
            }

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("me/preferences")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> UserPreferences()
        {
            var userId = GetCurrentUserId();

            var appUser = await userAccountService.GetUserByIdAsync(userId);
            if (appUser != null)
                return Ok(appUser.Configuration);

            return NotFound(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="UserAccountException"></exception>
        [HttpPut("me/preferences")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UserPreferences([FromBody] string data)
        {
            var userId = GetCurrentUserId();
            var appUser = await userAccountService.GetUserByIdAsync(userId);

            if (appUser != null)
            {
                appUser.Configuration = data;
                var result = await userAccountService.UpdateUserAsync(appUser);

                if (!result.Succeeded)
                {
                    throw new UserAccountException($"The following errors occurred whilst updating User Configurations: " +
                        $"{string.Join(", ", result.Errors)}");
                }

                return NoContent();
            }

            return NotFound(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        [ProducesResponseType(200, Type = typeof(UserVm))]
        public async Task<IActionResult> GetCurrentUser()
        {
            return await GetUserById(GetCurrentUserId());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUserById))]
        [ProducesResponseType(200, Type = typeof(UserVm))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (!(await authorizationService.AuthorizeAsync(User, id,
                UserAccountManagementOperations.ReadOperationRequirement)).Succeeded)
                return new ChallengeResult();

            var userVm = await GetUserViewModelHelper(id);

            return userVm != null ? Ok(userVm) : NotFound(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("username/{userName}")]
        [ProducesResponseType(200, Type = typeof(UserVm))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            var appUser = await userAccountService.GetUserByUserNameAsync(userName);

            if (!(await authorizationService.AuthorizeAsync(User, appUser?.Id ?? string.Empty,
                UserAccountManagementOperations.ReadOperationRequirement)).Succeeded)
                return new ChallengeResult();

            var userVm = appUser != null ? await GetUserViewModelHelper(appUser.Id) : null;

            return userVm != null ? Ok(userVm) : NotFound(userName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize(AuthPolicies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserVm>))]
        public async Task<IActionResult> GetUsers()
        {
            return await GetUsers(-1, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        [Authorize(AuthPolicies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserVm>))]
        public async Task<IActionResult> GetUsers(int pageNumber, int pageSize)
        {
            var usersAndRoles = await userAccountService.GetUsersAndRolesAsync(pageNumber, pageSize);

            var usersVm = new List<UserVm>();

            foreach (var item in usersAndRoles)
            {
                var userVm = _mapper.Map<UserVm>(item.User);
                userVm.Roles = item.Roles;

                usersVm.Add(userVm);
            }

            return Ok(usersVm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserEditVm user)
        {
            var userId = GetCurrentUserId($"Error retrieving the userId for user \"{user.UserName}\".");
            return await UpdateUser(userId, user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserEditVm user)
        {
            var appUser = await userAccountService.GetUserByIdAsync(id);
            var currentRoles = appUser != null
                ? (await userAccountService.GetUserRolesAsync(appUser)).ToArray() : null;

            var manageUsersPolicy = authorizationService.AuthorizeAsync(User, id,
                UserAccountManagementOperations.UpdateOperationRequirement);
            var assignRolePolicy = authorizationService.AuthorizeAsync(User, (user.Roles, currentRoles),
                AuthPolicies.AssignAllowedRolesPolicy);

            if ((await Task.WhenAll(manageUsersPolicy, assignRolePolicy)).Any(r => !r.Succeeded))
                return new ChallengeResult();

            if (appUser == null)
                return NotFound(id);

            if (!string.IsNullOrWhiteSpace(user.Id) && id != user.Id)
                AddModelError("Conflicting user id in parameter and model", nameof(id));

            var isNewPassword = !string.IsNullOrWhiteSpace(user.NewPassword);
            var isNewUserName = !appUser.UserName!.Equals(user.UserName, StringComparison.OrdinalIgnoreCase);

            if (GetCurrentUserId() == id)
            {
                if (string.IsNullOrWhiteSpace(user.CurrentPassword))
                {
                    if (isNewPassword)
                        AddModelError("Current password is required when changing your own password", "Password");

                    if (isNewUserName)
                        AddModelError("Current password is required when changing your own username", "Username");
                }
                else if (isNewPassword || isNewUserName)
                {
                    if (!await userAccountService.CheckPasswordAsync(appUser, user.CurrentPassword))
                        AddModelError("The username/password couple is invalid.");
                }
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);
            _mapper.Map(user, appUser);

            var result = await userAccountService.UpdateUserAsync(appUser, user.Roles);

            if (result.Succeeded)
            {
                if (isNewPassword)
                {
                    if (!string.IsNullOrWhiteSpace(user.CurrentPassword))
                        result = await userAccountService.UpdatePasswordAsync(appUser, user.CurrentPassword,
                            user.NewPassword!);
                    else
                        result = await userAccountService.ResetPasswordAsync(appUser, user.NewPassword!);
                }

                if (result.Succeeded)
                    return NoContent();
            }

            AddModelError(result.Errors);

            return BadRequest(ModelState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        [HttpPatch("me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] JsonPatchDocument<UserPatchVm> patch)
        {
            var userId = GetCurrentUserId();
            return await UpdateUser(userId, patch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] JsonPatchDocument<UserPatchVm> patch)
        {
            if (!(await authorizationService.AuthorizeAsync(User, id,
                UserAccountManagementOperations.UpdateOperationRequirement)).Succeeded)
                return new ChallengeResult();

            var appUser = await userAccountService.GetUserByIdAsync(id);
            if (appUser == null)
                return NotFound(id);

            var userPvm = _mapper.Map<UserPatchVm>(appUser);
            patch.ApplyTo(userPvm, e => AddModelError(e.ErrorMessage));

            if (!ModelState.IsValid) return BadRequest(ModelState);
            _mapper.Map(userPvm, appUser);

            var result = await userAccountService.UpdateUserAsync(appUser);

            if (result.Succeeded)
                return NoContent();

            AddModelError(result.Errors);

            return BadRequest(ModelState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<UserVm?> GetUserViewModelHelper(string userId)
        {
            var userAndRoles = await userAccountService.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
                return null;

            var userVm = _mapper.Map<UserVm>(userAndRoles.Value.User);
            userVm.Roles = userAndRoles.Value.Roles;

            return userVm;
        }
    }
}
