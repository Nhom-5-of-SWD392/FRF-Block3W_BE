using AutoMapper;
using Data.EFCore;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;

namespace Service.Core;

public interface ICommentService
{
    Task<IEnumerable<CommentResponseModel>> GetRepliesByCommentIdAsync(Guid parentCommentId);
    Task<string> DeleteCommentAsync(Guid commentId, string userId);
    Task<Guid> UpdateCommentAsync(Guid commentId, string userId, CommentUpdateModel model);

}

public class CommentService : ICommentService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public CommentService(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentResponseModel>> GetRepliesByCommentIdAsync(Guid parentCommentId)
    {
        try
        {
            var replies = await _dataContext.Comment
            .Where(c => !c.IsDeleted && c.ParentCommentId == parentCommentId)
            .Include(c => c.User)
            .Include(c => c.Reactions)
            .ToListAsync();

            return replies.Select(c => new CommentResponseModel(c));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<Guid> UpdateCommentAsync(Guid commentId, string userId, CommentUpdateModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException(ErrorMessage.Unauthorize);
            }

            var comment = await _dataContext.Comment
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == commentId);

            if (comment == null)
                throw new AppException(ErrorMessage.CommentNotFound);

            if (comment.User == null || comment.User.Id != new Guid(userId))
                throw new AppException(ErrorMessage.OnlyUpdateOwnComment);

            comment.Content = model.Content;

            comment.UpdatedBy = new Guid(userId);

            comment.UpdatedAt = DateTime.UtcNow;

            await _dataContext.SaveChangesAsync();

            return commentId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<string> DeleteCommentAsync(Guid commentId, string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException(ErrorMessage.Unauthorize);
            }

            var comment = await _dataContext.Comment
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == commentId);

            if (comment == null)
                throw new AppException(ErrorMessage.CommentNotFound);

            if (comment.User == null || comment.User.Id != new Guid(userId))
                throw new AppException(ErrorMessage.OnlyDeleteOwnComment);

            _dataContext.Comment.Remove(comment);

            await _dataContext.SaveChangesAsync();

            return "Deleted!";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
}
