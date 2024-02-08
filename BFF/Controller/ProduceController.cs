using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using BFF.Entity;

namespace BFF.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class ProduceController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ProduceController> _logger;

        public ProduceController(IConfiguration config, ILogger<ProduceController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult ProduceMessage([FromBody] ProduceRequest request)
        {

            _logger.LogInformation($"Received request to produce message. ID: {request.Id}, Message: {request.Message}");

            if (string.IsNullOrEmpty(request.Message))
            {
                _logger.LogError("Message is required."); // Log de erro se a mensagem estiver ausente
                return BadRequest("The message field is required.");
            }

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _config["RabbitMQ:HostName"],
                    UserName = _config["RabbitMQ:UserName"],
                    Password = _config["RabbitMQ:Password"]
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "messages",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var body = Encoding.UTF8.GetBytes($"ID: {request.Id}, Message: {request.Message}");

                    channel.BasicPublish(exchange: "",
                                         routingKey: "messages",
                                         basicProperties: null,
                                         body: body);

                    _logger.LogInformation("Message published successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to publish message to RabbitMQ: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, "Failed to publish message to RabbitMQ.");
            }

            return Ok("Message published successfully.");
        }
    }

}