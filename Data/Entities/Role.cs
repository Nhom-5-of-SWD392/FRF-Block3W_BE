
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Role : BaseEntities
    {
        [Required]
        public string? Name { get; set; }
        public bool? IsDefault { get; set; }

        // Foreign Keys
        public IList<PermissionAssignment> PermissionAssignment { get; set; } = null!;
    }
}
