using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class IngredientViewModel : BaseModel
{
	public string Name { get; set; } = string.Empty;
}

public class IngredientCreateModel
{
	public string Name { get; set; } = string.Empty;
	
}

public class IngredientUpdateModel
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
}

// PostIngredient View Model
public class IngredientDetailModel 
{
	[Required]
	public string Name { get; set; } = null!;
	public string Unit { get; set; } = string.Empty;
	public string Quantity { get; set; } = string.Empty;
}

public class IngredientQueryModel : QueryStringParameters
{
	public IngredientQueryModel()
	{
		OrderBy = "Name";
	}
	public string? Search { get; set; }
}

