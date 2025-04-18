using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;

namespace Service.Core;


public interface IMediaService
{
    Task<string> AddMediaAsync(Guid postId, IFormFile file);
    Task<string> RemoveMediaAsync(Guid mediaId);
}

public class MediaService : IMediaService
{
    private readonly DataContext _dataContext;
    private readonly ICloudinaryService _cloudinaryService;

    public MediaService(DataContext dataContext, ICloudinaryService cloudinaryService)
    {
        _dataContext = dataContext;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<string> AddMediaAsync(Guid postId, IFormFile file)
    {
        var post = await _dataContext.Post
            .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
        if (post == null)
            throw new AppException(ErrorMessage.PostNotFound);

        var contentType = file.ContentType.ToLower();
        MediaType mediaType;

        if (contentType.StartsWith("image/"))
        {
            mediaType = MediaType.Image;
        }
        else if (contentType.StartsWith("video/"))
        {
            mediaType = MediaType.Video;
        }
        else
        {
            throw new AppException(ErrorMessage.UnsupportedFile);
        }

        string path = mediaType == MediaType.Image ? $"{postId}/images" : $"{postId}/videos";
        string url = mediaType == MediaType.Image
            ? await _cloudinaryService.UploadImageAsync(file, path)
            : await _cloudinaryService.UploadVideoAsync(file, path);

        var media = new Media
        {
            Url = url,
            Type = mediaType,
            PostId = postId
        };

        await _dataContext.Media.AddAsync(media);
        await _dataContext.SaveChangesAsync();

        return url;
    }

    public async Task<string> RemoveMediaAsync(Guid mediaId)
    {
        var media = await _dataContext.Media
            .FirstOrDefaultAsync(m => m.Id == mediaId && !m.IsDeleted);
        if (media == null)
            throw new AppException(ErrorMessage.MediaNotFound);

        _dataContext.Media.Remove(media);
        await _dataContext.SaveChangesAsync();

        return "Remove successfully!";
    }
}
