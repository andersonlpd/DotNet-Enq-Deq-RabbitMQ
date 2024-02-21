using System.Text;

using OrchService.Config; 

namespace OrchService.PublishToMongoDB
{
    public class MongoDBPublisher
    {
        private readonly IConfiguration _config;
        private readonly LoggingConfig _loggingConfig;
        private readonly HttpClient _httpClient;

        public MongoDBPublisher(IConfiguration config, LoggingConfig loggingConfig)
        {
            _config = config;
            _loggingConfig = loggingConfig;
            _httpClient = new HttpClient();
        }

        public void Publish(string message)
        {
            try
            {
                var mongodbServiceUrl = _config["MongoDBService:Url"];
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                var response = _httpClient.PostAsync(mongodbServiceUrl, content).Result;
                response.EnsureSuccessStatusCode();

                _loggingConfig.LogInformation("Message published to MongoDB successfully.");
            }
            catch (Exception ex)
            {
                _loggingConfig.LogError($"Failed to publish message to MongoDB: {ex.Message}", ex);
            }
        }
    }
}