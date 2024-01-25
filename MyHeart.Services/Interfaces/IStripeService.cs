using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.User;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IPaymentGatewayProvider
    {
        Task<Customer> CreateCustomer(User user);
        Task<bool> UpdateCustomer(User user);
        Task<Session> CreateSession(CreateCheckoutSessionDTO createCheckoutSession, int userId);
        Task<StripeList<Product>> GetProductList(ProductListOptions options);
        Task<Product> GetProduct(string id, ProductGetOptions options);
        Task PaymentSucceeded(Invoice charge);
        Task LogPayment(Charge charge, int userId = 0);
        Task ResetSubscription(string customerId);
        Task<Stripe.Subscription> CancelSubscription(string stripeUserId);
        Task<Stripe.Subscription> GetActiveSubscription(string stripeUserId);
        Task<Stripe.Subscription> GetActiveSubscription(int userId);
    }
}