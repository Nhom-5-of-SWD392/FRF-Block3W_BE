using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class PostTopic : BaseEntities
{
    //Foreign Keys
    public Guid PostId { get; set; }
    [ForeignKey("PostId")]
    public Post? Post { get; set; }

    public Guid TopicId { get; set; }
    [ForeignKey("TopicId")]
    public Topic? Topic { get; set; }
}
