using Api.Filters;
using Application.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/seller")]
    public class SellerController : ControllerBase
    {
        private readonly SellerService _sellerService;
        private readonly StripeService _stripeService;
        private readonly IProductRepository _productRepository;

        public SellerController(
            SellerService sellerService,
            StripeService stripeService,
            IProductRepository productRepository)
        {
            _sellerService = sellerService;
            _stripeService = stripeService;
            _productRepository = productRepository;
        }

        [HttpGet("onboarding")]
        [ExtractUserId]  // 👈 خلي الفلتر يضيف الـ UserId
        public async Task<IActionResult> GetOnboardingUrl()
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;  // 👈 نفس اللي بتعمليه في ProfileController

            var url = await _sellerService.GetStripeOnboardingUrl(userId);

            return Ok(new { url });
        }


        // API لإرجاع بيانات السيلر بالكامل
        [HttpGet("{sellerId}")]
        public async Task<IActionResult> GetSeller(Guid sellerId)
        {
            var seller = await _sellerService.GetSellerByIdAsync(sellerId);
            if (seller == null)
                return NotFound("Seller not found");

            return Ok(new
            {
                seller.Id,
                seller.UserId,
                Email = seller.User.Email,
                seller.StripeAccountId
            });
        }

        // Pay to seller باستخدام ProductId
        [HttpGet("pay/{productId}")]
        public async Task<IActionResult> PayToSeller(Guid productId)
        {
            var product = await _sellerService.GetProductByIdAsync(productId, _productRepository);
            if (product == null)
                return NotFound("Product not found");

            var seller = await _sellerService.GetSellerByIdAsync(product.SellerId);
            if (seller == null)
                return NotFound("Seller not found");

            var price = (long)(product.Price * 100); // السعر بالسينتس

            var url = _stripeService.CreateCheckoutSessionUrl(seller.StripeAccountId, price, "usd");

            return Ok(new { url });
        }


    }
}
