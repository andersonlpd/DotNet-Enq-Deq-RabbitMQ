using System.Text;

using OrchService.Config; 

namespace OrchService.PublishToMySQL
{
    public class MySQLPublisher
    {
        private readonly IConfiguration _config;
        private readonly LoggingConfig _loggingConfig;
        private readonly HttpClient _httpClient;

        public MySQLPublisher(IConfiguration config, LoggingConfig loggingConfig)
        {
            _config = config;
            _loggingConfig = loggingConfig;
            _httpClient = new HttpClient();
        }

        public void Publish(string message)
        {
            try
            {
                var mysqlServiceUrl = _config["MySQLService:Url"];
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                var response = _httpClient.PostAsync(mysqlServiceUrl, content).Result;
                response.EnsureSuccessStatusCode();

                _loggingConfig.LogInformation("Message published to MySQL successfully.");
            }
            catch (Exception ex)
            {
                _loggingConfig.LogError($"Failed to publish message to MySQL: {ex.Message}", ex);
            }
        }
    }
}