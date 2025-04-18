using Data.Enum;
using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Post : BaseEntities
{
	[Required(ErrorMessage = "Title is required")]
	[StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
	public string Title { get; set; } = string.Empty;

	[Required(ErrorMessage = "Content is required")]
	public string Content { get; set; } = string.Empty;

    public PostStatus Status { get; set; } = PostStatus.Pending;

	//Foreign Keys
	public Guid PostById { get; set; }
	[ForeignKey("PostById")]
	public User? PostBy { get; set; }
    public Guid? ComfirmById { get; set; }
    [ForeignKey("ComfirmById")]
    public User? ConfirmBy { get; set; }

    public IList<PostIngredient>? PostIngredients { get; set; }
	public IList<PostTopic>? PostTopic { get; set; }
	public IList<Favorite>? Favorites { get; set; }
	public IList<Comment>? Comments { get; set; }
	public IList<Media>? Medias { get; set; }
	public IList<Instruction>? Instructions { get; set; }

}
