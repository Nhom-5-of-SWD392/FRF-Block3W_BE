using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models;

public class FavoriteViewModel : BaseModel
{
	public Guid? UserId { get; set; }
	public string UserName { get; set; }

	public List<PostViewModel> Posts { get; set; }
}

public class FavoriteCreateModel 
{
	
	public Guid UserId { get; set; }
	
	public Guid PostId { get; set; }


}

public class FavoriteQueryModel : QueryStringParameters
{
	public FavoriteQueryModel()
	{
		OrderBy = "CreatedAt";
	}
	public string? Search { get; set; }
}
