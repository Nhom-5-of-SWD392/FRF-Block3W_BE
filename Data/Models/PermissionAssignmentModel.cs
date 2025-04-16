
using System.Text.Json.Serialization;

namespace Data.Models;

public class PermissionAssignmentModel : BaseModel
{
    // Foreign Keys
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}

public class PermissionAssignmentViewModel : BaseModel
{

}

public class PermissionAssignmentCreateModel
{

}

public class PermissionAssignmentUpdateModel
{
    [JsonIgnore]
    public DateTime DateUpdate { get; set; } = DateTime.UtcNow;
}

public class PermissionAssignmentQueryModel : QueryStringParameters
{
    public PermissionAssignmentQueryModel()
    {

    }
    public string? Search { get; set; }
}
