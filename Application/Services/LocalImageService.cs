using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class LocalImageService : IImageService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private readonly string[] _allowedContentTypes = 
    { 
        "image/jpeg", 
        "image/png", 
        "image/gif", 
        "image/webp" 
    };

    public LocalImageService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        // Upload path: wwwroot/uploads/images
        _uploadPath = Path.Combine(environment.WebRootPath, "uploads", "images");
        
        // Ensure directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }

        // Base URL for accessing images
        _baseUrl = configuration["ImageService:BaseUrl"] ?? "/uploads/images";
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        if (!IsValidImage(fileName, contentType))
        {
            throw new InvalidOperationException("Invalid image file type");
        }

        if (imageStream.Length > _maxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {_maxFileSize / 1024 / 1024}MB");
        }

        // Generate unique filename
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        // Save file
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        // Return URL
        return $"{_baseUrl}/{uniqueFileName}";
    }

    public Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            // Extract filename from URL
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_uploadPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public bool IsValidImage(string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension) && _allowedContentTypes.Contains(contentType.ToLowerInvariant());
    }
}
