using Application.DTOs;
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
    public class ProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IImageService _imageService;

        public ProductService(IProductRepository productRepo, IImageService imageService)
        {
            _productRepo = productRepo;
            _imageService = imageService;
        }

        public async Task<List<ProductViewDto>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return products.Select(p => new ProductViewDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                SellerId = p.SellerId
            }).ToList();
        }

        public async Task<ProductViewDto?> GetProductByIdAsync(Guid id)
        {
            var p = await _productRepo.GetByIdAsync(id);
            if (p == null) return null;
            return new ProductViewDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                SellerId = p.SellerId
            };
        }

        public async Task<ProductViewDto> CreateProductAsync(ProductCreateDto dto, Guid sellerId, string? imageUrl)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                SellerId = sellerId,
                ImageUrl = imageUrl
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id)!;
        }

        public async Task<ProductViewDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto, string? imageUrl)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return null;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;

            if (dto.RemoveImage && !string.IsNullOrEmpty(product.ImageUrl))
            {
                await _imageService.DeleteImageAsync(product.ImageUrl);
                product.ImageUrl = null;
            }
            else if (imageUrl != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                    await _imageService.DeleteImageAsync(product.ImageUrl);
                product.ImageUrl = imageUrl;
            }

            await _productRepo.UpdateAsync(product);
            await _productRepo.SaveChangesAsync();

            return await GetProductByIdAsync(id);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return false;

            if (!string.IsNullOrEmpty(product.ImageUrl))
                await _imageService.DeleteImageAsync(product.ImageUrl);

            await _productRepo.DeleteAsync(product);
            await _productRepo.SaveChangesAsync();
            return true;
        }
    }
}
