namespace MongoDBApp.Config

{
    public class LoggingConfig
    {
        private readonly ILogger<LoggingConfig> _logger;

        public LoggingConfig(ILogger<LoggingConfig> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogInformation($"[{timestamp}] {message}");
        }

        public void LogError(string message, Exception ex)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogError($"[{timestamp}] {message}\n{ex.StackTrace}");
        }
    }
}