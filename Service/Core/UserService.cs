using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Utilities;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Service.Core;

public interface IUserService
{
    Task<JWTToken> Login(UserRequest model);
    //Task<JWTToken> LoginWithGoogle(string idToken);
    Task<PagingModel<UserViewModel>> GetAll(UserQueryModel query);
    Task<User> GetById(Guid id);
    Task<Guid> Create(UserCreateModel model);
    Task<Guid> Update(Guid id, UserUpdateModel model);
    Task<Guid> Delete(Guid id);
    Task<string> ChangePasswordAsync(string userId, ChangePasswordModel model);
    Task<string> RequestPasswordResetAsync(PasswordResetRequestModel model);
    Task<string> ResetPasswordAsync(PasswordResetModel passwordResetmodel);
    Task<Guid> RegisterAsync(RegisterUserModel model);
    Task<Guid> RegisterModeratorAsync(string userId);
    Task<string> ProcessModeratorApplicationAsync(string confirmedId, Guid requestId, ModeratorApplicationApproveModel model);
}
public class UserService : IUserService
{
    private readonly DataContext _dataContext;
    private ISortHelpers<User> _sortHelper;
    private readonly IMapper _mapper;
    private readonly IJwtUtils _jwtUtils;
    private readonly IConfiguration _configuration;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IEmailService _emailService;
    private readonly ICloudinaryService _cloudinaryService;
    //private readonly IFilterHelper<User> _filterHelperUser;


    public UserService(
            DataContext dataContext, 
            ISortHelpers<User> sortHelper, 
            IMapper mapper, 
            IConfiguration configuration, 
            IJwtUtils jwtUtils, 
            IGoogleAuthService googleAuthService,
            IEmailService emailService,
            ICloudinaryService cloudinaryService)
    {
        _dataContext = dataContext;
        _sortHelper = sortHelper;
        _mapper = mapper;
        _configuration = configuration;
        _jwtUtils = jwtUtils;
        _googleAuthService = googleAuthService;
        _emailService = emailService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<JWTToken> Login(UserRequest model)
    {
        try
        {
            var user = await _dataContext.User
                .Where(x => !x.IsDeleted && x.UserName == model.UserName)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new AppException(ErrorMessage.InvalidAccount);
            }
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                throw new AppException(ErrorMessage.InvalidAccount);
            }

            var authClaims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user?.FirstName + " " + user?.LastName ?? ""),
                new Claim(ClaimTypes.Email, user?.Email ?? ""),
                new Claim(ClaimTypes.Role, user?.Role.ToString() ?? ""),
                new Claim("avartar", user?.AvatarUrl ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = _jwtUtils.GenerateToken(authClaims, _configuration.GetSection("JWT").Get<JwtModel>(), user);

            _dataContext.User.Update(user);
            await _dataContext.SaveChangesAsync();

            return token;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    //public async Task<JWTToken> LoginWithGoogle(string idToken)
    //{
    //    try
    //    {
    //        var authenticateResult = await _googleAuthService.ValidateGoogleToken(idToken);
    //        if (authenticateResult == null)
    //        {
    //            throw new AppException(ErrorMessage.AccessTokenFail);
    //        }
    //        var user = await _dataContext.User
    //            .Where(i => i.Email == authenticateResult.Email || i.GoogleId == authenticateResult.Subject && !i.IsDeleted)
    //            .FirstOrDefaultAsync();

    //        if (user == null)
    //        {
    //            var newUser = new UserCreateModel
    //            {
    //                Email = authenticateResult.Email,
    //                FirstName = authenticateResult.GivenName ?? "",
    //                LastName = authenticateResult.FamilyName ?? "",
    //                AvatarUrl = authenticateResult.Picture ?? "",
    //                Role = UserRole.Member,
    //                UserName = authenticateResult.Email.Split('@')[0],
    //                IsModerator = false,
    //                Gender = Gender.Other,
    //                GoogleId = authenticateResult.Subject
    //            };
    //            var createUser = await Create(newUser);
    //        }

    //        var authClaims = new List<Claim>
    //        {
    //            new Claim("id", user.Id.ToString()),
    //            new Claim(ClaimTypes.Name, user?.FirstName + " " + user?.LastName ?? ""),
    //            new Claim(ClaimTypes.Email, user?.Email ?? ""),
    //            new Claim(ClaimTypes.Role, user?.Role.ToString() ?? ""),
    //            new Claim("avartar", user?.AvatarUrl ?? ""),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //        };

    //        var token = _jwtUtils.GenerateToken(authClaims, _configuration.GetSection("JWT").Get<JwtModel>(), user);

    //        return token;
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e);
    //        throw new AppException(e.Message);
    //    }
    //}
    public async Task<User> GetById(Guid id)
    {
        try
        {
            var user = await _dataContext.User
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Id == id);
            if (user == null)
            {
                throw new AppException(ErrorMessage.UserNotFound);
            }

            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<Guid> Update(Guid id, UserUpdateModel model)
    {
        try
        {
            var data = await GetById(id);
            if (data == null)
            {
                throw new AppException(ErrorMessage.IdNotExist);
            }
            var updateData = _mapper.Map(model, data);

            _dataContext.User.Update(updateData);

            await _dataContext.SaveChangesAsync();
            return data.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<Guid> Delete(Guid id)
    {
        var data = await GetById(id);

        data.IsDeleted = true;
        _dataContext.User.Update(data);
        await _dataContext.SaveChangesAsync();
        return data.Id;
    }

    public async Task<string> ChangePasswordAsync(string userId, ChangePasswordModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException(ErrorMessage.Unauthorize);
            }

            var user = await _dataContext.User.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            if (user == null)
            {
                throw new AppException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
            {
                throw new AppException("Current password is incorrect.");
            }

            if (!IsValid(model.NewPassword))
                throw new AppException(
                    "Password does not meet the required complexity standards:\n" +
                    "- At least 8 characters long\n" +
                    "- Include UPPERCASE and lowercase letters\n" +
                    "- At least one digit\n" +
                    "- At least one special character @#$%^&*!_"
                );

            if (model.NewPassword != model.ConfirmPassword)
            {
                throw new AppException("New password and confirm password do not match.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            _dataContext.User.Update(user);

            await _dataContext.SaveChangesAsync();

            return "Change password successfully.";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<string> RequestPasswordResetAsync(PasswordResetRequestModel model)
    {
        try
        {
            var user = await _dataContext.User.FirstOrDefaultAsync(u => u.Email == model.Email && !u.IsDeleted);
            if (user == null)
                throw new AppException("Email does not exist.");

            var token = Guid.NewGuid().ToString();
            var resetLink = $"https://intern-s.vercel.app/reset-password?email={model.Email}&token={token}";

            user.ForgotPwdToken = token;
            user.ForgotPwdTokenExpiration = DateTime.UtcNow.AddHours(24);

            await _dataContext.SaveChangesAsync();

            await _emailService.SendResetPasswordEmailAsync(user, model.Email, resetLink);

            return resetLink;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<string> ResetPasswordAsync(PasswordResetModel passwordResetModel)
    {
        try
        {
            var user = await _dataContext.User.FirstOrDefaultAsync(u => u.Email == passwordResetModel.Email && !u.IsDeleted);

            if (user == null || user.ForgotPwdToken != passwordResetModel.Token)
                throw new AppException("Invalid token or email.");

            if (user.ForgotPwdToken != passwordResetModel.Token || user.ForgotPwdTokenExpiration < DateTime.UtcNow)
                throw new AppException("Invalid or expired token.");


            if (!IsValid(passwordResetModel.NewPassword))
                throw new AppException(
                    "Password does not meet the required complexity standards:\n" +
                    "- At least 8 characters long\n" +
                    "- Include UPPERCASE and lowercase letters\n" +
                    "- At least one digit\n" +
                    "- At least one special character @#$%^&*!_"
                );

            user.Password = BCrypt.Net.BCrypt.HashPassword(passwordResetModel.NewPassword);

            user.ForgotPwdToken = null;

            await _dataContext.SaveChangesAsync();

            return "Password reset successful.";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<Guid> RegisterAsync(RegisterUserModel model)
    {
        if (model.Password != model.ConfirmPassword)
            throw new AppException("Passwords do not match.");

        var existingUser = await _dataContext.User.FirstOrDefaultAsync(x => x.Email == model.Email || x.UserName == model.UserName);
        if (existingUser != null)
            throw new AppException("Email or username already exists.");

        string avatarUrl = "https://t4.ftcdn.net/jpg/05/49/98/39/360_F_549983970_bRCkYfk0P6PP5fKbMhZMIb07mCJ6esXL.jpg";

        string path = model.UserName + "/Avatar";

        if (model.AvatarUrl != null)
        {
            avatarUrl = await _cloudinaryService.UploadImageAsync(model.AvatarUrl, path);
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            UserName = model.UserName,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Dob = DateTime.SpecifyKind(model.Dob, DateTimeKind.Utc),
            Gender = model.Gender,
            Bio = model.Bio,
            Address = model.Address,
            AvatarUrl = avatarUrl,
            Role = UserRole.Member
        };

        await _dataContext.User.AddAsync(user);
        await _dataContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task<Guid> RegisterModeratorAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new AppException(ErrorMessage.Unauthorize);
        }

        var existing = await _dataContext.ModeratorApplication
            .FirstOrDefaultAsync(x => x.RegisterById == new Guid(userId) && x.Status == ApplicationStatus.Pending);

        if (existing != null) 
            throw new AppException(ErrorMessage.AlreadyApplyModerator);

        var application = new ModeratorApplication
        {
            RegisterById = new Guid(userId),
            Status = ApplicationStatus.Pending
        };

        await _dataContext.ModeratorApplication.AddAsync(application);

        await _dataContext.SaveChangesAsync();

        return application.Id;
    }

    public async Task<string> ProcessModeratorApplicationAsync(string confirmedId, Guid requestId, ModeratorApplicationApproveModel model)
    {
        if (string.IsNullOrEmpty(confirmedId))
        {
            throw new AppException(ErrorMessage.Unauthorize);
        }

        var request = await _dataContext.ModeratorApplication
            .Include(x => x.Registrant)
            .FirstOrDefaultAsync(x => x.Id == requestId);

        if (request == null) throw new AppException(ErrorMessage.RequestNotFound);

        if (request.Status != ApplicationStatus.Pending)
            throw new AppException(ErrorMessage.RequestAlreadyProcessed);

        request.ConfirmedById = new Guid(confirmedId);
        request.UpdatedBy = new Guid(confirmedId);
        request.Status = model.IsApproved ? ApplicationStatus.Approved : ApplicationStatus.Rejected;

        if (model.IsApproved)
        {
            var user = request.Registrant!;
            user.IsModerator = true;
        }

        await _dataContext.SaveChangesAsync();

        return "Approved!";
    }


    public bool IsValid(string password)
    {
        if (password.Length < 8) return false;

        if (!Regex.IsMatch(password, @"[A-Z]")) return false;

        if (!Regex.IsMatch(password, @"[a-z]")) return false;

        if (!Regex.IsMatch(password, @"\d")) return false;

        if (!Regex.IsMatch(password, @"[@#$%^&*!_]")) return false;

        return true;
    }

    public Task<PagingModel<UserViewModel>> GetAll(UserQueryModel query)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> Create(UserCreateModel model)
    {
        throw new NotImplementedException();
    }
}
