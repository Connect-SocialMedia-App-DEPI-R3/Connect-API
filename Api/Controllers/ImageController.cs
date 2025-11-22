using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/images")]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    // POST: api/images/upload
    [Authorize]
    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded" });
        }

        try
        {
            if (!_imageService.IsValidImage(file.FileName, file.ContentType))
            {
                return BadRequest(new { message = "Invalid image file. Only JPG, PNG, GIF, and WebP are allowed." });
            }

            using var stream = file.OpenReadStream();
            var imageUrl = await _imageService.UploadImageAsync(stream, file.FileName, file.ContentType);

            return Ok(new { imageUrl });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while uploading the image", error = ex.Message });
        }
    }

    // DELETE: api/images?url={imageUrl}
    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> DeleteImage([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest(new { message = "Image URL is required" });
        }

        var result = await _imageService.DeleteImageAsync(url);

        if (!result)
        {
            return NotFound(new { message = "Image not found" });
        }

        return NoContent();
    }
}
