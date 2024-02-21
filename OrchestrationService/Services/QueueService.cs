using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

using OrchService.Config;
using OrchService.PublishToMySQL;
using OrchService.PublishToMongoDB;

namespace OrchService.Queue
{
    public class RabbitMQOrchestrator
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

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _config["RabbitMQ:QueueName"],
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Parse message to determine ID
                var id = int.Parse(message.Split(':')[0]);

                try
                {
                    if (id % 2 == 0)
                    {
                        _mySQLPublisher.Publish(message);
                    }
                    else
                    {
                        _mongoDBPublisher.Publish(message);
                    }
                }
                catch (Exception ex)
                {
                    _loggingConfig.LogError($"Failed to process message: {ex.Message}", ex);
                }
            };

            _channel.BasicConsume(queue: _config["RabbitMQ:QueueName"],
                                  autoAck: true,
                                  consumer: consumer);

            _loggingConfig.LogInformation("Orchestrator started listening to RabbitMQ queue.");
        }
    }
}