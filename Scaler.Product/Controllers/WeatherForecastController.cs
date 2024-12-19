using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scaler.Core.Authorization;

namespace Scaler.Product.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class WeatherForecastController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _authorizationService = authorizationService;
        }
        [Authorize]
        [HttpGet(Name = "GetWeatherForecast")]
        [ProducesResponseType(200, Type = typeof(List<WeatherForecast>))]
        public async Task<IActionResult> GetAsync()
        {
            if (!(await _authorizationService.AuthorizeAsync(User, string.Empty,
                UserAccountManagementOperations.ReadOperationRequirement)).Succeeded)
            {
                ChallengeResult challengeResult = new ChallengeResult();
                return challengeResult;
            }
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToList();
            return Ok(result);
        }
    }
}
