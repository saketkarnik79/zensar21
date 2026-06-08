using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP_DemoWebAPIJWTAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        //[Authorize(Roles = "Admin")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("2", Name = "GetWeatherForecast2")]
        //[Authorize(Roles = "User")]
        public IEnumerable<WeatherForecast> Get2()
        {
            return Enumerable.Range(1, 3).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("all")]
        [Authorize]
        public IActionResult GetAll()
        {
            return Ok("Authenticated access");
        }

        [HttpGet("read")]
        [Authorize(Policy = "CanRead")]
        public IActionResult Read()
        {
            return Ok("Read Access Granted");
        }

        [HttpPost("write")]
        [Authorize(Policy = "CanWrite")]
        public IActionResult Write()
        {
            return Ok("Write Access Granted");
        }

        [HttpDelete("delete")]
        [Authorize(Policy = "CanDelete")]
        public IActionResult Delete()
        {
            return Ok("Delete Access Granted");
        }

        [HttpDelete("admin-delete")]
        [Authorize(Policy = "AdminOnlyWithDelete")]
        public IActionResult AdminDelete()
        {
            return Ok("Admin Delete Access Granted");
        }
    }
}
