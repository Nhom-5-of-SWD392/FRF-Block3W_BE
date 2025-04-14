
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public class PermissionModel : BaseModel
    {
        [Required]
        public string Resource { get; set; } = null!;

    }

    public class PermissionViewModel : BaseModel
    {
        [Required]
        public string Resource { get; set; } = null!;
    }

    public class PermissionCreateModel
    {
        [Required]
        public string Resource { get; set; } = null!;
    }

    public class PermissionUpdateModel
    {
        [Required]
        public string Resource { get; set; } = null!;
        [JsonIgnore]
        public DateTime DateUpdate { get; set; } = DateTime.UtcNow;
    }

    public class PermissionQueryModel : QueryStringParameters
    {
        public PermissionQueryModel()
        {

        }
        public string? Search { get; set; }
    }
}
