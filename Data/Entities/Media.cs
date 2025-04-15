using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class Media : BaseEntities
	{
		[Required(ErrorMessage = "Media Url is required")]
		public string Url { get; set; } = null!;
		[Required(ErrorMessage = "Media Type is required")]
		public string Type { get; set; } = null!;

		//Mình không biết Ref là gì nên mình để đại, nhờ kiểm tra sau
		[Required(ErrorMessage = "Media Ref is required")]
		public string Ref_Id { get; set; } = null!;

		[Required(ErrorMessage = "Media Ref Type is required")]
		public string RefType { get; set; } = null!;

		//Foreign Keys
		public Guid PostId { get; set; }
		
		[ForeignKey("PostId")]
		public Post? Post { get; set; }
		
	}
	
	
}
