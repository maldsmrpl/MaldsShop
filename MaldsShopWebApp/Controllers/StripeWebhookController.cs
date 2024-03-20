using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
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
        private readonly StripeSettings _stripeSettings;
        private readonly IUserRepository _userRepository;

        public StripeWebhookController(IOptions<StripeSettings> stripeSettings, IUserRepository userRepository)
        {
            _stripeSettings = stripeSettings.Value;
            _userRepository = userRepository;
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

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        _userRepository.UpdateLastActivity(User.Identity.Name);
                        return Ok();
                    }
                }

                return Ok("Event received but not processed");
            }
            catch (Exception e)
            {
                return BadRequest("Error processing the event");
            }
        }

    }
}
