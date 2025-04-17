using Data.Enum;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Data.Models;

public class UserViewModel : BaseModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public bool IsModerator { get; set; }
}

public class UserCreateModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime Dob { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string? Bio { get; set; }
    public string? Address { get; set; }
    public string AvatarUrl { get; set; } = "https://t4.ftcdn.net/jpg/05/49/98/39/360_F_549983970_bRCkYfk0P6PP5fKbMhZMIb07mCJ6esXL.jpg";
    public string? ForgotPwdToken { get; set; }
    public UserRole Role { get; set; }
    public bool? IsModerator { get; set; }
}

public class UserUpdateModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public DateTime Dob { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Bio { get; set; }
    public string? Address { get; set; }
    [JsonIgnore]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class UserQueryModel : QueryStringParameters
{
    public UserQueryModel()
    {
        OrderBy = "Name";
    }
    public string? Search { get; set; }
    public Guid? RoleId { get; set; }
}

public class UserRequest
{
    [Required(ErrorMessage = "Username is required")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}

public class ChangePasswordModel
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
    [Required]
    public string NewPassword { get; set; } = string.Empty;
    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class PasswordResetRequestModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}

public class PasswordResetModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}

public class RegisterUserModel
{
    [Required(ErrorMessage = "Name is required")]
    [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Only letters are allowed")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Name is required")]
    [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Only letters are allowed")]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and have 10 digits")]
    public string Phone { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public DateTime Dob { get; set; }
    public Gender Gender { get; set; }
}

