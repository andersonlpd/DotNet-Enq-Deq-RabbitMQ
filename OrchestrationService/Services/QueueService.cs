using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using OrchService.Config;
using OrchService.PublishToMySQL;
using OrchService.PublishToMongoDB;

namespace OrchService.Queue
{
    public class RabbitMQOrchestrator : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly LoggingConfig _loggingConfig;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly MySQLPublisher _mySQLPublisher;
        private readonly MongoDBPublisher _mongoDBPublisher;

        public RabbitMQOrchestrator(IConfiguration config, LoggingConfig loggingConfig,
                                    MySQLPublisher mySQLPublisher, MongoDBPublisher mongoDBPublisher)
        {
            _config = config;
            _loggingConfig = loggingConfig;
            _mySQLPublisher = mySQLPublisher;
            _mongoDBPublisher = mongoDBPublisher;

            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:HostName"],
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"]
            };

            _loggingConfig.LogInformation("Starting connection to RabbitMQ");

            try {

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(queue: _config["RabbitMQ:QueueName"],
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            } 
            catch (Exception ex) 
            {
                _loggingConfig.LogError($"Failed to connect to RabbitMQ: {ex.Message}", ex);
            }

            _loggingConfig.LogInformation($"Orchestrator started listening to RabbitMQ successfully.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try {
                
                _loggingConfig.LogDebug("Starting to consume messages...");
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _loggingConfig.LogDebug($"Got message from queue {message}");

                    // Parse message to determine ID and message content
                    var id = int.Parse(message.Split(':')[0]);
                    var content = message.Split(':')[1];

                    _loggingConfig.LogDebug($"message parsed id: {id}, content: {content}");

                    // Generate current timestamp in epoch format
                    var epochTimestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                    // Create new message with timestamp
                    var newMessage = $"{{\"timestamp\": {epochTimestamp}, \"id\": {id}, \"message\": \"{content}\"}}";

                    _loggingConfig.LogDebug($"New message generated: {newMessage}");

                    if (id % 2 == 0)
                    {
                        _mySQLPublisher.Publish(newMessage);
                        _loggingConfig.LogInformation("Message sent successfully to MySQL persistence service");
                    }
                    else
                    {
                        _mongoDBPublisher.Publish(newMessage);
                        _loggingConfig.LogInformation("Message sent successfully to MongoDB persistence service");
                    }

                    _loggingConfig.LogDebug($"Message read from queue");
                    _channel.BasicConsume(queue: _config["RabbitMQ:QueueName"],
                                            autoAck: true,
                                            consumer: consumer);
                };
            }
            catch (Exception ex) 
            {
                _loggingConfig.LogError($"Failed to consume message from RabbitMQ: {ex.Message}", ex);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _loggingConfig.LogInformation($"Worker ativo em: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                await Task.Delay(5000, stoppingToken);
            }

        }
    }
}