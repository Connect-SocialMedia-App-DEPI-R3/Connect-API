using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class StripeService
    {
        private readonly string _secretKey;

        public StripeService(IConfiguration configuration)
        {
            _secretKey = configuration["StripeSettings:SecretKey"];
            StripeConfiguration.ApiKey = _secretKey;
        }

        // لإنشاء حساب Stripe Express
        public async Task<string> CreateStripeAccountAsync(string email)
        {
            var options = new AccountCreateOptions
            {
                Type = "express",
                Email = email
            };
            var service = new AccountService();
            var account = await service.CreateAsync(options);
            return account.Id; // هذا الـ StripeAccountId
        }

        // لإنشاء رابط onboarding للمستخدم
        public string CreateAccountLink(string stripeAccountId)
        {
            var options = new AccountLinkCreateOptions
            {
                Account = stripeAccountId,
                RefreshUrl = "https://connect-depi.vercel.app/market",
                ReturnUrl = "https://connect-depi.vercel.app/market",
                Type = "account_onboarding"
            };
            var service = new AccountLinkService();
            var accountLink = service.Create(options);
            return accountLink.Url; // ترجّع URL للفرونت
        }



        public string CreateCheckoutSessionUrl(string sellerStripeAccountId, long amount, string currency = "usd")
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = amount,
                    Currency = currency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Product Payment"
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = "https://connect-depi.vercel.app/market",
                CancelUrl = "https://connect-depi.vercel.app/market"
            };

            var service = new SessionService();
            var session = service.Create(options, new RequestOptions
            {
                StripeAccount = sellerStripeAccountId
            });

            return session.Url; // 🔹 URL مباشر للصفحة
        }


    }

    }
