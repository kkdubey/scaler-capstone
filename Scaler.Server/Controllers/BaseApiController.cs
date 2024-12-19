using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Scaler.Core.Services;
using Scaler.Core.Services.Account;
using Scaler.Server.Attributes;

namespace Scaler.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [SanitizeModel]
    public class BaseApiController(ILogger<BaseApiController> logger, IMapper mapper) : ControllerBase
    {
        protected readonly IMapper _mapper = mapper;
        protected readonly ILogger<BaseApiController> _logger = logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        protected string GetCurrentUserId(string errorMsg = "Error retrieving the userId for the current user.")
        {
            return Utilities.GetUserId(User) ?? throw new UserNotFoundException(errorMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="key"></param>
        protected void AddModelError(IEnumerable<string> errors, string key = "")
        {
            foreach (var error in errors)
            {
                AddModelError(error, key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="key"></param>
        protected void AddModelError(string error, string key = "")
        {
            ModelState.AddModelError(key, error);
        }
    }
}
