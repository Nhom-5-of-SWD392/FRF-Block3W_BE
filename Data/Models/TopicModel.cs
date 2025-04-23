using Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Models;

public class TopicViewModel : BaseModel
{
    public string ImageUrl { get; set; } = string.Empty;
    [Required]
	public string? Name { get; set; }
}

public class TopicCreateModel
{
	[Required]
	public string? Name { get; set; }
}

public class TopicUpdateModel
{
	[Required]
	public string? Name { get; set; }

	[JsonIgnore]
	public DateTime DateUpdate { get; set; } = DateTime.UtcNow;
}

public class TopicQueryModel : QueryStringParameters
{
	public TopicQueryModel()
	{
		OrderBy = "Name";
	}

	public string? Search { get; set; }
}

public class TopicQueryPostModel : QueryStringParameters
{
    public TopicQueryPostModel()
    {
        OrderBy = "CreateAt";
    }

    public string? Search { get; set; }
    public PostStatus? Status { get; set; }
}

public class TopicAddToPostModel
{
    public Guid Id { get; set; }
}

public class PostTopicResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
    public List<PostResponse> Posts { get; set; } = new();
}

public class PostResponse
{
	public string? Media { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
}

