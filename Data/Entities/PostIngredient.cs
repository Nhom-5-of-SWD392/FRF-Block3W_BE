using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class PostIngredient : BaseEntities
	{
		[Required(ErrorMessage = "Ingredient Unit (g,kg,ml) is required")]
		public string Unit { get; set; } = null!;

		//TODO: Không biết có nên đổi thành int không
		[Required(ErrorMessage = "Ingredient Quantity is required")]
		public string Quantity { get; set; } = null!;

		//Foreign Keys
		public Guid PostId { get; set; }
		[ForeignKey("PostId")]
		public Post? Post { get; set; }
		public Guid IngredientId { get; set; }

		[ForeignKey("IngredientId")]
		public Ingredient? Ingredient { get; set; }
	}
}
