using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SellerService
    {
        private readonly ISellerRepository _sellerRepo;
        private readonly IUserRepository _userRepo;
        private readonly StripeService _stripeService;

        public SellerService(ISellerRepository sellerRepo, IUserRepository userRepo, StripeService stripeService)
        {
            _sellerRepo = sellerRepo;
            _userRepo = userRepo;
            _stripeService = stripeService;
        }

        public async Task<string> GetStripeOnboardingUrl(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var seller = await _sellerRepo.GetByUserIdAsync(userId);

            // لو أول مرة يدخل يعمل onboarding
            if (seller == null)
            {
                string stripeAccountId = await _stripeService.CreateStripeAccountAsync(user.Email);

                seller = new Seller
                {
                    UserId = userId,
                    StripeAccountId = stripeAccountId
                };

                await _sellerRepo.AddAsync(seller);
                await _sellerRepo.SaveAsync();
            }

            // بعد ما نتاكد ان له حساب → ندي له اللينك
            return _stripeService.CreateAccountLink(seller.StripeAccountId);
        }

        public async Task<Seller?> GetSellerByIdAsync(Guid sellerId)
        {
            return await _sellerRepo.GetByIdAsync(sellerId);
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId, IProductRepository productRepo)
        {
            return await productRepo.GetByIdAsync(productId);
        }

    }
}
