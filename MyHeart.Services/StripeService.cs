using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.Services.Interfaces;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class StripeService : IPaymentGatewayProvider
    {
        private IMapper _mapper;
        private ILogger<StripeService> _logger;
        private IStripeConfiguration _stripeConfiguration;

        private MyHeartContext _dbContext;
        private CustomerService _customerService;
        private SessionService _sessionService;
        private ProductService _productService;
        private Stripe.SubscriptionService _subscriptionService;

        const int defaultSubscriptionId = 1;

        public StripeService(MyHeartContext dbContext, CustomerService customerService, IMapper mapper, ILogger<StripeService> logger,
            SessionService sessionService, IStripeConfiguration stripeConfiguration, ProductService productService, Stripe.SubscriptionService subscriptionService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
            _mapper = mapper;
            _logger = logger;
            _sessionService = sessionService;
            _stripeConfiguration = stripeConfiguration;
            _productService = productService;
            _subscriptionService = subscriptionService;
        }

        public async Task<Customer> CreateCustomer(User user)
        {
            var options = _mapper.Map<CustomerCreateOptions>(user);

            try
            {
                var customer = await _customerService.CreateAsync(options);
                return customer;
            }
            catch (StripeException sx)
            {
                _logger.LogError(sx, "Stripe customer creation failed");
                return null;
            }


        }

        public async Task<bool> UpdateCustomer(User user)
        {
            var options = _mapper.Map<CustomerUpdateOptions>(user);

            try
            {
                await _customerService.UpdateAsync(user.StripeId, options);
                return true;
            }
            catch (StripeException sx)
            {
                _logger.LogError(sx, "Stripe customer creation failed");
                return false;
            }
        }

        public async Task<Session> CreateSession(CreateCheckoutSessionDTO createCheckoutSession, int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);

            var subscriptionOptions = new SubscriptionListOptions
            {
                Customer = user.StripeId,
                Status = "active",
                Limit = 1
            };

            var activeSubscriptions = _subscriptionService.List(subscriptionOptions);
            if (activeSubscriptions.Data.Count > 0)
            {
                return null;
            }

            var productOptions = new ProductGetOptions
            {
                Expand = new List<string> { "default_price" }
            };

            var product = await GetProduct(createCheckoutSession.ProductStripeId, productOptions);

            if (product == null)
                return null;

            var successUrl = _stripeConfiguration.SuccessUrl.Replace("{userId}", $"{user.Id}");
            var cancelledUrl = _stripeConfiguration.CancelledUrl.Replace("{userId}", $"{user.Id}");

            var sessionOptions = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = product.DefaultPriceId,
                        Quantity = 1,
                    }
                },
                SuccessUrl = successUrl,
                CancelUrl = cancelledUrl,
                Mode = "subscription",
                Locale = "cs",
                Customer = user.StripeId
            };

            try
            {
                var session = await _sessionService.CreateAsync(sessionOptions);
                return session;
            }
            catch (StripeException sx)
            {
                _logger.LogError(sx, "Stripe session creation failed");
                return null;
            }

        }

        public async Task<Product> GetProduct(string id, ProductGetOptions options)
        {
            try
            {
                var product = await _productService.GetAsync(id, options);
                return product;
            }
            catch (StripeException sx)
            {
                _logger.LogError(sx, "Fetching product failed");
                return null;
            }
        }

        public async Task<StripeList<Product>> GetProductList(ProductListOptions options)
        {
            try
            {
                var products = await _productService.ListAsync(options);
                return products;
            }
            catch (StripeException sx)
            {
                _logger.LogError(sx, "Fetching product failed");
                return null;
            }
        }

        public async Task PaymentSucceeded(Invoice invoice)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserDetail)
                .FirstOrDefaultAsync(u => u.StripeId == invoice.CustomerId);

            if (user == null)
            {
                _logger.LogError($"Cant process invoice {invoice.Id} for customerid {invoice.CustomerId}, User not found");
                return;
            }

            //always one product
            var item = invoice.Lines.FirstOrDefault();

            var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.ProductStripeId == item.Price.ProductId);
            if (subscription == null)
            {
                _logger.LogError($"Cant process invoice {invoice.Id} for customerid {invoice.CustomerId}, Product {item.Price.ProductId} is not assigned to subsription not found");
                return;
            }

            var stripeSubscription = await _subscriptionService.GetAsync(invoice.SubscriptionId);

            user.UserDetail.SubscriptionId = subscription.Id;
            user.UserDetail.SubscriptionValidTo = stripeSubscription.CurrentPeriodEnd;

            await _dbContext.SaveChangesAsync();
        }

        public async Task ResetSubscription(string customerId)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserDetail)
                .FirstOrDefaultAsync(u => u.StripeId == customerId);

            if (user == null)
            {
                _logger.LogError($"Cant reset subscirption for customerid {customerId}, User not found");
                return;
            }

            user.UserDetail.SubscriptionId = defaultSubscriptionId;
            user.UserDetail.SubscriptionValidTo = null;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Stripe.Subscription> CancelSubscription(string stripeUserId)
        {
            var activeSubscription = await GetActiveSubscription(stripeUserId);

            if (activeSubscription == null || activeSubscription.CancelAtPeriodEnd)
                return null;

            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = true
            };

            var subscription = await _subscriptionService.UpdateAsync(activeSubscription.Id, options);

            return subscription;
        }

        public async Task LogPayment(Charge charge, int userId = 0)
        {
            if (userId == 0)
            {
                var user = await _dbContext.Users
                     .FirstOrDefaultAsync(u => u.StripeId == charge.CustomerId);

                userId = user.Id;
            }


            var log = new PaymentLog
            {
                CustomerId = charge.CustomerId,
                Amount = charge.Amount,
                CreatedDate = DateTime.Now,
                Currency = charge.Currency,
                InvoiceId = charge.InvoiceId,
                ChargeId = charge.Id,
                UserId = userId,
                PaymentMethod = charge.PaymentMethod,
                State = charge.Status,
                FailureReason = charge.FailureMessage
            };

            _dbContext.Add(log);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Stripe.Subscription> GetActiveSubscription(string stripeUserId)
        {
            var subscriptionOptions = new SubscriptionListOptions
            {
                Customer = stripeUserId,
                Status = "active",
                Limit = 1
            };

            StripeList<Stripe.Subscription> activeSubscriptions = await _subscriptionService.ListAsync(subscriptionOptions);
            Stripe.Subscription activeSubscription = activeSubscriptions.Data.FirstOrDefault();

            return activeSubscription;
        }


        public async Task<Stripe.Subscription> GetActiveSubscription(int userId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                var subscriptionOptions = new SubscriptionListOptions
                {
                    Customer = user.StripeId,
                    Status = "active",
                    Limit = 1
                };

                StripeList<Stripe.Subscription> activeSubscriptions = await _subscriptionService.ListAsync(subscriptionOptions);
                Stripe.Subscription activeSubscription = activeSubscriptions.Data.FirstOrDefault();

                return activeSubscription;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveSubscription");
                return null;
            }
            
        }
    }
}
