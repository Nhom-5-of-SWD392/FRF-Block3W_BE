using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Utilities;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Service.Core;

public interface IUserService
{
    Task<JWTToken> Login(UserRequest model);
    //Task<JWTToken> LoginWithGoogle(string idToken);
    Task<PagingModel<UserViewModel>> GetAll(UserQueryModel query);
    Task<UserViewModel> GetById(Guid id);
    Task<Guid> Create(UserCreateModel model);
    Task<Guid> Update(Guid id, UserUpdateModel model);
    Task<Guid> Delete(Guid id);
    Task<string> ChangePasswordAsync(string userId, ChangePasswordModel model);
    Task<string> RequestPasswordResetAsync(PasswordResetRequestModel model);
    Task<string> ResetPasswordAsync(PasswordResetModel passwordResetmodel);
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
    //private readonly IFilterHelper<User> _filterHelperUser;


    public UserService(
            DataContext dataContext, 
            ISortHelpers<User> sortHelper, 
            IMapper mapper, 
            IConfiguration configuration, 
            IJwtUtils jwtUtils, 
            IGoogleAuthService googleAuthService,
            IEmailService emailService)
    {
        _dataContext = dataContext;
        _sortHelper = sortHelper;
        _mapper = mapper;
        _configuration = configuration;
        _jwtUtils = jwtUtils;
        _googleAuthService = googleAuthService;
        _emailService = emailService;
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

    public async Task<Guid> Update(Guid id, UserUpdateModel model)
    {
        try
        {
            var data = await GetUser(id);
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
        var data = await GetUser(id);
        if (data == null)
        {
            throw new AppException(ErrorMessage.IdNotExist);
        }
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

    private void SearchByKeyWord(ref IQueryable<User> users, string keyword)
    {
        if (!users.Any() || string.IsNullOrWhiteSpace(keyword))
            return;
        users = users.Where(o => o.UserName.ToLower().Contains(keyword.Trim().ToLower()) || o.Email.ToLower().Contains(keyword.Trim().ToLower()));
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

    private async Task<User> GetUser(Guid id)
    {
        try
        {
            var data = await _dataContext.User
                .Where(x => !x.IsDeleted && x.Id == id)
                .SingleOrDefaultAsync();
            if (data == null) throw new AppException(ErrorMessage.IdNotExist);
            return data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public Task<PagingModel<UserViewModel>> GetAll(UserQueryModel query)
    {
        throw new NotImplementedException();
    }

    public Task<UserViewModel> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> Create(UserCreateModel model)
    {
        throw new NotImplementedException();
    }
}
