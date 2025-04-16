using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Topic : BaseEntities
{
	[Required(ErrorMessage = "Topic Name is required")]
	public string Name { get; set; } = null!;
	
	public IList<PostTopic> PostTopics { get; set; }
}
