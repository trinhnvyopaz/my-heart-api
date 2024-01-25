using MyHeart.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Data;
using MyHeart.Services;
using System.Threading.Tasks;
using System.Collections;
using MyHeart.DTO;
using Stripe;
using System.IO;
using System;
using Stripe.Checkout;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private IPaymentGatewayProvider _paymentGatewayProvider;
        private IUserService _userService;
        private IStripeConfiguration _stripeConfiguration;
        private ChargeService _chargeService;

        public PaymentGatewayController(IPaymentGatewayProvider paymentGatewayProvider, IUserService userService, IStripeConfiguration stripeConfiguration, ChargeService chargeService)
        {
            _paymentGatewayProvider = paymentGatewayProvider;
            _userService = userService;
            _stripeConfiguration = stripeConfiguration;
            _chargeService = chargeService;
        }

        [HttpPost("PaymentHook")]
        public async Task<IActionResult> PaymentHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _stripeConfiguration.WebhookSecret);

                // Handle the event
                if (stripeEvent.Type == Events.InvoicePaid)
                {
                    var charge = stripeEvent.Data.Object as Invoice;
                    await _paymentGatewayProvider.PaymentSucceeded(charge);
                }
                else if(stripeEvent.Type == Events.ChargeSucceeded || stripeEvent.Type == Events.ChargeFailed || stripeEvent.Type == Events.ChargeRefunded)
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    await _paymentGatewayProvider.LogPayment(charge);
                }
                else if(stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    await _paymentGatewayProvider.ResetSubscription(subscription.CustomerId);
                }
                else if(stripeEvent.Type == Events.InvoicePaymentFailed)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    await _paymentGatewayProvider.ResetSubscription(invoice.CustomerId);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e);
            }
        }


        [HttpPost("createCheckoutSession")]
        public async Task<ActionResult<Session>> GetSessionAsync(CreateCheckoutSessionDTO createCheckoutSession)
        {
            var currentUser = await _userService.CurrentUserAsync();

            var session = await _paymentGatewayProvider.CreateSession(createCheckoutSession, currentUser.Id);
            if (session == null)
            {
                return BadRequest();
            }

            return Ok(session);
        }

        [HttpPut("cancelSubscription")]
        public async Task<ActionResult<Subscription>> CancelSubscription(CancelSubscriptionRequest model)
        {
            var currentUser = await _userService.CurrentUserAsync();

            if(currentUser.StripeId != model.StripeUserId)            
                return BadRequest();
            

            var subscription = await _paymentGatewayProvider.CancelSubscription(model.StripeUserId);
            if (subscription == null)
                return BadRequest();

            return Ok(subscription);
        }
    }
}