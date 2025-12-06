using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly IImageService _imageService;

        public ProductController(ProductService productService, IImageService imageService)
        {
            _productService = productService;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost("{sellerId}")]
        public async Task<IActionResult> Create(Guid sellerId, [FromForm] ProductCreateDto dto, IFormFile? file)
        {
            string? imageUrl = null;
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                imageUrl = await _imageService.UploadImageAsync(stream, file.FileName, file.ContentType);
            }

            var product = await _productService.CreateProductAsync(dto, sellerId, imageUrl);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] ProductUpdateDto dto, IFormFile? file)
        {
            string? imageUrl = null;
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                imageUrl = await _imageService.UploadImageAsync(stream, file.FileName, file.ContentType);
            }

            var product = await _productService.UpdateProductAsync(id, dto, imageUrl);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
