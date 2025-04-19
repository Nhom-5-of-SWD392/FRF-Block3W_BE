using Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models;

public class ReactionViewModel : BaseModel
{
	public ReactionType? ReactionType { get; set; }
	public Guid UserId { get; set; }
	public Guid CommentId { get; set; }
}

public class ReactionCreateModel
{
	public ReactionType? ReactionType { get; set; }
	public Guid UserId { get; set; }
	public Guid CommentId { get; set; }
}
