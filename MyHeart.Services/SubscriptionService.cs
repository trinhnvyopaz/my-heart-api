using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.DTO;
using MyHeart.DTO.User;
using MyHeart.Services.Interfaces;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private MyHeartContext _dbContext;
        private IMapper _mapper;
        private IPaymentGatewayProvider _paymentGatewayProvider;

        public SubscriptionService(MyHeartContext dbContext, IMapper mapper, IPaymentGatewayProvider paymentGatewayProvider)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _paymentGatewayProvider = paymentGatewayProvider;
        }

        public async Task<SubscriptionDTO> GetSubscription(int id)
        {
            var dbSubscription = await _dbContext.Subscriptions.FindAsync(id);
            if (dbSubscription == null)
                return null;

            var options = new ProductGetOptions
            {
                Expand = new List<string> { "default_price" }
            };

            var product = await _paymentGatewayProvider.GetProduct(dbSubscription.ProductStripeId, options);

            var subsription = _mapper.Map<SubscriptionDTO>(dbSubscription);

            if (product != null)
            {
                subsription.Name = product.Name;
                subsription.Price = (product.DefaultPrice.UnitAmount ?? 0) / 100;
                subsription.ProductStripeId = subsription.ProductStripeId;
            }

            return subsription;
        }

        public async Task<IEnumerable<SubscriptionDTO>> GetSubscriptions()
        {
            var list = await _dbContext.Subscriptions.ToListAsync();
            
            var options = new ProductListOptions
            {
                Ids = list.Select(i => i.ProductStripeId).ToList(),
                Expand = new List<string> { "data.default_price" },
            };

            var productList = await _paymentGatewayProvider.GetProductList(options);

            var subscriptionDtoList = new List<SubscriptionDTO>();

            var sortedProducts = productList.Data.OrderBy(x => x.DefaultPrice.UnitAmount).ToList();

            foreach (var product in sortedProducts)
            {
                var subsription = list.FirstOrDefault(i => i.ProductStripeId == product.Id);

                subscriptionDtoList.Add(new SubscriptionDTO
                {
                    Id = subsription.Id,
                    LastUpdateDate = subsription.LastUpdateDate,
                    LastUpdateUser = subsription.LastUpdateUser,
                    Name = product.Name,
                    Price = (product.DefaultPrice.UnitAmountDecimal ?? 0) / 100,
                    ProductStripeId = subsription.ProductStripeId,
                });
            }

            return subscriptionDtoList;
        }
    }
}
