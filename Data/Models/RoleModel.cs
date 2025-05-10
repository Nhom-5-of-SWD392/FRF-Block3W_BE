
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Models;

public class RoleViewModel : BaseModel
{
    [Required]
    public string? Name { get; set; }
}

public class RoleCreateModel
{
    [Required]
    public string? Name { get; set; }
}

public class RoleUpdateModel
{
    [Required]
    public string? Name { get; set; }
    [JsonIgnore]
    public DateTime DateUpdate { get; set; } = DateTime.UtcNow;
}

public class RoleQueryModel : QueryStringParameters
{
    public RoleQueryModel()
    {

    }
    public string? Search { get; set; }
}
