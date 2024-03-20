using MaldsShopWebApp.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace MaldsShopWebApp.Services
{

    public class WebhookProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WebhookProcessingService> _logger;

        public WebhookProcessingService(IServiceProvider serviceProvider, ILogger<WebhookProcessingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var taskQueue = _serviceProvider.GetRequiredService<IBackgroundTaskQueue>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogInformation($"Error while ExecuteAsync: {exception}");
                }
            }
        }
    }
}
