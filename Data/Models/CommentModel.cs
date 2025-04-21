using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class CommentCreateModel
{
    [Required]
    public string Content { get; set; } = string.Empty;

    public Guid? ParentCommentId { get; set; }
}

public class CommentUpdateModel
{
    [Required]
    public string Content { get; set; } = string.Empty;
}


public class CommentResponseModel
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ParentCommentId { get; set; }

    public CommentResponseModel(Comment comment)
    {
        Id = comment.Id;
        Content = comment.Content;
        FullName = comment.User?.FirstName + " " + comment.User?.LastName ?? "Anonymous";
        AvatarUrl = comment.User?.AvatarUrl ?? "";
        CreatedAt = comment.CreatedAt;
        ParentCommentId = comment.ParentCommentId;
    }
}


