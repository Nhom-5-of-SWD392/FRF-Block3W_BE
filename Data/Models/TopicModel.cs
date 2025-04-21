using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Models;

public class TopicViewModel : BaseModel
{
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

public class TopicAddToPostModel
{
    public Guid Id { get; set; }
}

public class PostTopicResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public List<PostResponse> Posts { get; set; } = new();
}

public class PostResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
}

