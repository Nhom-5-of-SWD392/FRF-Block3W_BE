using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Utilities;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Service.Core
{
    public interface IUserService
    {
        Task<JWTToken> Login(UserRequest model);
        Task<PagingModel<UserViewModel>> GetAll(UserQueryModel query);
        Task<UserViewModel> GetById(Guid id);
        Task<Guid> Create(UserCreateModel model);
        Task<Guid> Update(Guid id, UserUpdateModel model);
        Task<Guid> Delete(Guid id);
        Task<string> ChangePasswordAsync(string userId, ChangePasswordModel model);
    }
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private ISortHelpers<User> _sortHelper;
        private readonly IMapper _mapper;
        private readonly IJwtUtils _jwtUtils;
        private readonly IConfiguration _configuration;
        //private readonly IFilterHelper<User> _filterHelperUser;


        public UserService(DataContext dataContext, ISortHelpers<User> sortHelper, IMapper mapper, IConfiguration configuration, IJwtUtils jwtUtils)
        {
            _dataContext = dataContext;
            _sortHelper = sortHelper;
            _mapper = mapper;
            _configuration = configuration;
            _jwtUtils = jwtUtils;
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

                var getRole = await _dataContext.Role
                    .Where(x => !x.IsDeleted && x.Id == user.RoleId)
                    .FirstOrDefaultAsync();

                var authClaims = new List<Claim>
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user?.FullName ?? ""),
                    new Claim(ClaimTypes.Email, user?.Email ?? ""),
                    new Claim("avartar", user?.Avatar ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, getRole.Name.ToString()),
                };

                var token = _jwtUtils.GenerateToken(authClaims, _configuration.GetSection("JWT").Get<JwtModel>(), user, getRole);

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
                    throw new AppException("User is not authorized.");
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

        private void SearchByKeyWord(ref IQueryable<User> users, string keyword)
        {
            if (!users.Any() || string.IsNullOrWhiteSpace(keyword))
                return;
            users = users.Where(o => o.FullName.ToLower().Contains(keyword.Trim().ToLower()) || o.UserName.ToLower().Contains(keyword.Trim().ToLower()) || o.Email.ToLower().Contains(keyword.Trim().ToLower()));
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
}
