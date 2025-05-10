using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Ingredient : BaseEntities
{
	[Required(ErrorMessage = "Ingredient Name is required")]
	public string Name { get; set; } = string.Empty;
}
