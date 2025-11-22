namespace Application.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Uploads an image and returns the URL
    /// </summary>
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);

    /// <summary>
    /// Deletes an image by its URL or filename
    /// </summary>
    Task<bool> DeleteImageAsync(string imageUrl);

    /// <summary>
    /// Validates if the file is a valid image
    /// </summary>
    bool IsValidImage(string fileName, string contentType);
}
