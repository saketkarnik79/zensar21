using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await ConsumeEvent();

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ConsumeEvent()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = await factory.CreateConnectionAsync()) 
            {
                using (var channel = await connection.CreateChannelAsync())
                {
                    await channel.QueueDeclareAsync("ordersq", false, false, false);
                    var consumer = new AsyncEventingBasicConsumer(channel);

                    consumer.ReceivedAsync += Consumer_ReceivedAsync;

                    await channel.BasicConsumeAsync("ordersq", true, consumer);
                }
            }
        }

        private Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            var message = Encoding.UTF8.GetString(@event.Body.ToArray());
            Console.WriteLine($"Received: {message}");
            return Task.CompletedTask;
        }
    }
}
