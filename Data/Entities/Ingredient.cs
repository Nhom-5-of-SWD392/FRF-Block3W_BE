using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class Ingredient : BaseEntities
	{
		[Required(ErrorMessage = "Ingredient Name is required")]
		public string Name { get; set; } = null!;
	}
}
