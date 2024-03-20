using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace MaldsShopWebApp.Controllers
{
    [Route("api/stripe")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly StripeSettings _stripeSettings;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingCartRepository _shippingCartRepository;
        private readonly IServiceScopeFactory _scopeFactory;

        public StripeWebhookController(
            ILogger<StripeWebhookController> logger,
            IOptions<StripeSettings> stripeSettings,
            IBackgroundTaskQueue taskQueue,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IShippingCartRepository shippingCartRepository,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _stripeSettings = stripeSettings.Value;
            _taskQueue = taskQueue;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _shippingCartRepository = shippingCartRepository;
            _scopeFactory = scopeFactory;
        }

        [HttpPost("listen")]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeSettings.WebHookSecret);

                _logger.LogInformation($"Received Stripe event: {stripeEvent.Type}");

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    _taskQueue.QueueBackgroundWorkItem(async token =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                            var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                            var shippingCartRepository = scope.ServiceProvider.GetRequiredService<IShippingCartRepository>();

                            await ProcessEventAsync(stripeEvent, userRepository, productRepository, orderRepository, shippingCartRepository);
                        }
                    });

                    return Ok();
                }

                return Ok("Event received but not processed");
            }
            catch (Exception e) // Catch a more general exception to log all errors
            {
                _logger.LogError($"Error processing Stripe event: {e.Message}");
                return BadRequest();
            }
        }

        private async Task ProcessEventAsync(Event stripeEvent, IUserRepository userRepository, IProductRepository productRepository, IOrderRepository orderRepository, IShippingCartRepository shippingCartRepository)
        {
            _logger.LogInformation($"Processing Stripe event: {stripeEvent.Type}");

            var session = stripeEvent.Data.Object as Session;
            if (session != null)
            {
                var sessionId = session.Id;
                Order order = new Order();
                try
                {
                    order = await orderRepository.FindByStripeSessionIdAsync(sessionId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                if (order != null)
                {
                    var user = await userRepository.GetByEmailAsync(session.CustomerEmail);
                    if (user != null)
                    {
                        order.IsPaid = true;
                        order.StripeInvoiceId = session.PaymentIntentId.ToString();
                        await orderRepository.UpdateAsync(order);

                        foreach (var item in user.ShippingCart.ShippingCartItems)
                        {
                            await productRepository.ProductSoldAsync(item.Product, item.Quantity);
                        }

                        await userRepository.UpdateLastActivityAsync(user.Email);

                        await shippingCartRepository.ClearShippingCart(user.ShippingCart);
                    }
                    else
                    {
                        Console.WriteLine($"User with email: {session.CustomerEmail} not found");
                    }
                    
                }
                else
                {
                    Console.WriteLine($"Order for StripeSessionId: {session.Id} not found");
                }
            }
        }
    }
}
