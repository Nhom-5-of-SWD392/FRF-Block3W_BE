using Data.Entities;
using Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models;

public class PostViewModel : BaseModel
{
	public string? Title { get; set; }
	public PostStatus? Status { get; set; }
	public Guid PostById { get; set; }
	public Guid? ConfirmBy { get; set; }
	//Change to TopicViewModel
	public List<PostTopic> Topics { get; set; } = new();
}

public class PostCreateModel
{
	public string? Title { get; set; }
	public string? Content { get; set; }
	public List<TopicAddToPostModel> Topics { get; set; } = new();
}

public class PostDetailModel : BaseModel
{
	public string? Title { get; set; }
	public string? Content { get; set; }
	public PostStatus? Status { get; set; }
	public Guid PostById { get; set; }
	public Guid ConfirmBy { get; set; }
	public List<PostIngredient> Ingredients { get; set; } = new();
	public List<PostTopic> Topics { get; set; } = new();
}

public class PostQueryModel : QueryStringParameters
{
	public PostQueryModel()
	{
		
	}

	public string? Search { get; set; }
}

public class PostDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PostByName { get; set; } = string.Empty;

    public List<IngredientDetail> Ingredients { get; set; } = new();
    public List<string> Topics { get; set; } = new();
    public List<MediaResponse> MediaUrls { get; set; } = new();
    public List<InstructionResponse> Instructions { get; set; } = new();
}

public class IngredientDetail
{
	public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}

public class MediaResponse
{
	public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
}

public class InstructionResponse
{
	public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
