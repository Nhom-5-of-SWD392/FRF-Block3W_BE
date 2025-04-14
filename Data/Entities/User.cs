using Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Entities
{
    public class User : BaseEntities
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(50, ErrorMessage = "User Name can't be longer than 50 characters")]
        public string UserName { get; set; } = null!;
        [Required]
        [JsonIgnore]
        public string Password { get; set; } = null!;
        [Required]
        public Gender Gender { get; set; }
        [Phone(ErrorMessage = "Phone is not true to the format")]
        [StringLength(13, ErrorMessage = "Phone number up to 13 characters long")]
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public bool? IsActive { get; set; }

        // Foreign Keys
        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role? Roles { get; set; }
    }
}
