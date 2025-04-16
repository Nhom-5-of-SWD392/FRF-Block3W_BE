using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class PostIngredient : BaseEntities
{
	public string Unit { get; set; } = string.Empty;
	public string Quantity { get; set; } = string.Empty;

    //Foreign Keys
    public Guid PostId { get; set; }
	[ForeignKey("PostId")]
	public Post? Post { get; set; }

	public Guid IngredientId { get; set; }
	[ForeignKey("IngredientId")]
	public Ingredient? Ingredient { get; set; }
}
