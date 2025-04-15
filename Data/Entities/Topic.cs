using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class Topic : BaseEntities
	{
		[Required(ErrorMessage = "Topic Name is required")]
		public string Name { get; set; } = null!;
	}
}
