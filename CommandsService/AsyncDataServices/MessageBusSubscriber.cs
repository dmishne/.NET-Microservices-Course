namespace CommandsServices.AsyncDataServices
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandsService.Config;
    using CommandsService.EventProcessing;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class MessageBusSubscriber : BackgroundService, IDisposable
    {
        private const string Exchange = "trigger";
        private readonly MessageQueueConfig _messageQueueConfig;
        private readonly IEventProcessor _eventProcessor;
        private readonly ILogger<MessageBusSubscriber> _logger;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IEventProcessor eventProcessor, IOptions<MessageQueueConfig> messageQueueOption, ILogger<MessageBusSubscriber> logger)
        {
            _messageQueueConfig = messageQueueOption.Value;
            _eventProcessor = eventProcessor;
            _logger = logger;

            Initialize();
        }

        public void Initialize()
        {
            var factory = new ConnectionFactory
            {
                HostName = _messageQueueConfig.Host,
                Port = _messageQueueConfig.Port
            };
            try
            {
                _connection = factory.CreateConnection();
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName, exchange: Exchange, routingKey: string.Empty);

                _logger.LogInformation("Listening to MessageBus");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MessageBusClient: Count not connect to the message bus");
            }
        }

        public override void Dispose()
        {
            _logger.LogInformation("MessageBusClient disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                _logger.LogInformation("Event Received!");

                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("RabbitMQ connection shutdown");
        }
    }
}