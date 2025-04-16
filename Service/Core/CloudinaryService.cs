using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Service.Utilities;

namespace Service.Core;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string path);
    Task<string> UploadVideoAsync(IFormFile file, string path);
}


public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
    }

    public async Task<string> UploadImageAsync(IFormFile file, string path)
    {
        if (file == null || file.Length == 0)
            throw new AppException("No file uploaded.");

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = path
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.SecureUrl.ToString();
    }

    public async Task<string> UploadVideoAsync(IFormFile file, string path)
    {
        if (file == null || file.Length == 0)
            throw new AppException(ErrorMessage.NoVideo);

        await using var stream = file.OpenReadStream();
        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = path
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.SecureUrl.ToString();
    }
}


