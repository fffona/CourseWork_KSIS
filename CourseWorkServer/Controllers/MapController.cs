using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseWorkServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MapController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("api-key")]
        public IActionResult GetApiKey()
        {
            var apiKey = _configuration["YandexMaps:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, "API-ключ не настроен");
            return Ok(new { ApiKey = apiKey });
        }
    }
}
