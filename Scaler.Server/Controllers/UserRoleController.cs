using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scaler.Core.Authorization;
using Scaler.Core.Models.Account;
using Scaler.Core.Services.Account;
using Scaler.Server.ViewModels.Account;
using System.Data;

namespace Scaler.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/account")]
    [Authorize]
    public class UserRoleController(
        ILogger<UserRoleController> logger,
        IMapper mapper,
        IUserRoleService userRoleService,
        IAuthorizationService authorizationService)
        : BaseApiController(logger, mapper)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost("roles")]
        [Authorize(AuthPolicies.ManageAllRolesPolicy)]
        [ProducesResponseType(201, Type = typeof(RoleVm))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRole([FromBody] RoleVm? role)
        {
            if (role == null)
                return BadRequest($"{nameof(role)} cannot be null");

            var appRole = _mapper.Map<ApplicationRole>(role);

            var result = await userRoleService
                .CreateRoleAsync(appRole, role.Permissions?.Select(p => p.Value!).ToArray() ?? []);

            if (result.Succeeded)
            {
                var roleVm = await GetRoleViewModelHelper(appRole.Name!);
                return CreatedAtAction(nameof(GetRoleById), new { id = roleVm?.Id }, roleVm);
            }

            AddModelError(result.Errors);

            return BadRequest(ModelState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserRoleException"></exception>
        [HttpDelete("roles/{id}")]
        [Authorize(AuthPolicies.ManageAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(RoleVm))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var appRole = await userRoleService.GetRoleByIdAsync(id);

            if (appRole == null)
                return NotFound(id);

            var canDelete = await userRoleService.TestCanDeleteRoleAsync(id);
            if (!canDelete.Success)
            {
                AddModelError($"Role \"{appRole.Name}\" cannot be deleted at this time. " +
                    "Delete the associated user and try again");
                AddModelError(canDelete.Errors, "Records");
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var roleVm = await GetRoleViewModelHelper(appRole.Name!, false);
            var result = await userRoleService.DeleteRoleAsync(appRole);

            if (!result.Succeeded)
            {
                throw new UserRoleException($"Following errors occurred whilst deleting role \"{id}\": " +
                                            $"{string.Join(", ", result.Errors)}");
            }

            return Ok(roleVm);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("permissions")]
        [Authorize(AuthPolicies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PermissionVm>))]
        public IActionResult GetAllPermissions()
        {
            return Ok(_mapper.Map<List<PermissionVm>>(ApplicationPermissions.AllPermissions));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("roles/{id}", Name = nameof(GetRoleById))]
        [ProducesResponseType(200, Type = typeof(RoleVm))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var applicationRole = await userRoleService.GetRoleByIdAsync(id);

            if (!(await authorizationService.AuthorizeAsync(User, applicationRole?.Name ?? string.Empty,
                AuthPolicies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();

            var roleVm = applicationRole != null ? await GetRoleViewModelHelper(applicationRole.Name!) : null;

            return roleVm != null ? Ok(roleVm) : NotFound(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("roles/name/{name}")]
        [ProducesResponseType(200, Type = typeof(RoleVm))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            if (!(await authorizationService.AuthorizeAsync(User, name,
                AuthPolicies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();

            var roleVm = await GetRoleViewModelHelper(name);

            return roleVm != null ? Ok(roleVm) : NotFound(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles")]
        [Authorize(AuthPolicies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RoleVm>))]
        public async Task<IActionResult> GetRoles()
        {
            return await GetRoles(-1, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("roles/{pageNumber:int}/{pageSize:int}")]
        [Authorize(AuthPolicies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RoleVm>))]
        public async Task<IActionResult> GetRoles(int pageNumber, int pageSize)
        {
            var roles = await userRoleService.GetRolesLoadRelatedAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<List<RoleVm>>(roles));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPut("roles/{id}")]
        [Authorize(AuthPolicies.ManageAllRolesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleVm? role)
        {
            if (role == null)
                return BadRequest($"{nameof(role)} cannot be null");

            if (!string.IsNullOrWhiteSpace(role.Id) && id != role.Id)
                return BadRequest("Conflicting role id in parameter and model data");

            var applicationRole = await userRoleService.GetRoleByIdAsync(id);

            if (applicationRole == null)
                return NotFound(id);

            _mapper.Map(role, applicationRole);

            var result = await userRoleService
                .UpdateRoleAsync(applicationRole, role.Permissions?.Select(p => p.Value!).ToArray());

            if (result.Succeeded)
                return NoContent();

            AddModelError(result.Errors);

            return BadRequest(ModelState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="loadRelatedEntities"></param>
        /// <returns></returns>
        private async Task<RoleVm?> GetRoleViewModelHelper(string roleName, bool loadRelatedEntities = true)
        {
            var applicationRole = loadRelatedEntities ? await userRoleService.GetRoleLoadRelatedAsync(roleName)
                : await userRoleService.GetRoleByNameAsync(roleName);

            return applicationRole != null ? _mapper.Map<RoleVm>(applicationRole) : null;
        }
    }
}
