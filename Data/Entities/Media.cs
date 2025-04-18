using Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Media : BaseEntities
{
	[Required(ErrorMessage = "Media Url is required")]
	public string Url { get; set; } = string.Empty;
	[Required(ErrorMessage = "Media Type is required")]
	public MediaType Type { get; set; }

    //Foreign Keys
    public Guid PostId { get; set; }
	[ForeignKey("PostId")]
	public Post? Post { get; set; }
}


