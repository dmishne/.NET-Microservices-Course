namespace PlatformService.AsyncDataServices
{
    using System;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using PlatformService.Config;
    using PlatformService.Dtos;
    using RabbitMQ.Client;

    public class MessageBusClient : IMessageBusClient, IDisposable
    {
        private const string Exchange = "trigger";
        private readonly MessageQueueConfig _messageQueueConfig;
        private readonly ILogger<MessageBusClient> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IOptions<MessageQueueConfig> messageQueueOption, ILogger<MessageBusClient> logger)
        {
            _messageQueueConfig = messageQueueOption.Value;
            _logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = _messageQueueConfig.Host,
                Port = _messageQueueConfig.Port
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                _logger.LogInformation("Connected to MessageBus");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MessageBusClient: Count not connect to the message bus");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishDto)
        {
            var message = JsonSerializer.Serialize(platformPublishDto);

            if (_connection.IsOpen)
            {
                _logger.LogInformation("RabbitMQ connection open. Sending message...");
                SendMessage(message);
            }
            else
            {
                _logger.LogInformation("RabbitMQ connection is closed. Not seding message...");
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("MessageBusClient disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: Exchange, routingKey: string.Empty, basicProperties: null, body: body);

            _logger.LogInformation($"Sent message: {message}");
        }


        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("RabbitMQ connection shutdown");
        }
    }
}