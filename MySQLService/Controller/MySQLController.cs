using Microsoft.AspNetCore.Mvc;

using MySQLApp.Services;
using MySQLApp.Entity;
using MySQLApp.Config;

namespace MySQLApp.Controllers
{
    [ApiController]
    [Route("/persist/mysql")]
    public class MySQLController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MySQLService _mySQLService;
        private readonly LoggingConfig _loggingConfig;

        public MySQLController(IConfiguration config, MySQLService mySQLService, LoggingConfig loggingConfig)
        {
            _config = config;
            _mySQLService = mySQLService;
            _loggingConfig = loggingConfig;
        }

        [HttpPost]
        public IActionResult PersistMessage([FromBody] PersistRequest request)
        {
            try
            {
                _mySQLService.PersistMessage(request);
                _loggingConfig.LogInformation("Message persisted in MySQL successfully.");
                return Ok("Message persisted in MySQL successfully.");
            }
            catch (Exception ex)
            {
                _loggingConfig.LogError($"Failed to persist message in MySQL: {ex.Message}", ex);
                return StatusCode(500, "Failed to persist message in MySQL.");
            }
        }
    }
}