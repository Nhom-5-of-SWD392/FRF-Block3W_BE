using Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class User : BaseEntities
{
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(50, ErrorMessage = "First Name can't be longer than 50 characters")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(50, ErrorMessage = "Last Name can't be longer than 50 characters")]
    public string LastName { get; set; } = string.Empty;
    [Phone(ErrorMessage = "Phone is not true to the format")]
    [StringLength(13, ErrorMessage = "Phone number up to 13 characters long")]
    public string? Phone { get; set; }
    public DateTime Dob { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(50, ErrorMessage = "User Name can't be longer than 50 characters")]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public Gender Gender { get; set; }
    public string? Bio { get; set; }
    public string? Address { get; set; }
    public string AvatarUrl { get; set; } = "https://t4.ftcdn.net/jpg/05/49/98/39/360_F_549983970_bRCkYfk0P6PP5fKbMhZMIb07mCJ6esXL.jpg";
    public string? ForgotPwdToken { get; set; }
    public UserRole Role { get; set; } = UserRole.Member;
    public bool? IsModerator { get; set; }
    public string? GoogleId { get; set; }

    // Foreign Keys
    public IList<ModeratorApplication>? RequestRegister { get; set; }
    public IList<ModeratorApplication>? Comfirmed { get; set; }
    public IList<Notification>? Notifications { get; set; }
    public IList<Reaction>? Reactions { get; set; }
    public IList<Comment>? Comments { get; set; }
    public IList<Post>? Posts { get; set; }
    public IList<Post>? ApprovedPosts { get; set; }
    public IList<Favorite>? Favorites { get; set; }
    public IList<QuizResult>? QuizMade { get; set; }
    public IList<QuizResult>? QuizEvaluate { get; set; }
}
