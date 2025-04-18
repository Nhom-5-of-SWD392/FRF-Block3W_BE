using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;

namespace Service.Core;


public interface IMediaService
{
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
