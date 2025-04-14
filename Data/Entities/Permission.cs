
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Permission : BaseEntities
    {
        [Required]
        public string? Resource { get; set; }

    }
}
