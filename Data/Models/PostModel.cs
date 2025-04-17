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
	public List<PostTopic> Topics { get; set; } = new();
}

public class PostCreateModel
{
	public string? Title { get; set; }
	public string? Content { get; set; }
	public List<IngredientDetailModel> Ingredients { get; set; } = new();
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