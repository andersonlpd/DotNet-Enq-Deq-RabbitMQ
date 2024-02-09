using Microsoft.AspNetCore.Mvc;

using MongoDBApp.Services;
using MongoDBApp.Entity;
using MongoDBApp.Config;

namespace MongoDBApp.Controllers
{
    [ApiController]
    [Route("/persist/mongodb")]
    public class MongoDBController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MongoDBService _MongoDBService;
        private readonly LoggingConfig _loggingConfig;

        public MongoDBController(IConfiguration config, MongoDBService MongoDBService, LoggingConfig loggingConfig)
        {
            _config = config;
            _MongoDBService = MongoDBService;
            _loggingConfig = loggingConfig;
        }

        [HttpPost]
        public IActionResult PersistMessage([FromBody] PersistRequest request)
        {
            try
            {
                _MongoDBService.PersistMessage(request);
                _loggingConfig.LogInformation("Message persisted in MongoDB successfully.");
                return Ok("Message persisted in MongoDB successfully.");
            }
            catch (Exception ex)
            {
                _loggingConfig.LogError($"Failed to persist message in MongoDB: {ex.Message}", ex);
                return StatusCode(500, "Failed to persist message in MongoDB.");
            }
        }
    }
}