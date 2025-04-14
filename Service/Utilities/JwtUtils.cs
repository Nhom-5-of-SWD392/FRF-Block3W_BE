using Data.Entities;
using Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Utilities
{
    public interface IJwtUtils
    {
        public JWTToken GenerateToken(IEnumerable<Claim> claims, JwtModel? jwtModel, User user, Role role);
    }

    public class JwtUtils : IJwtUtils
    {
        public JwtUtils()
        {

        }

        public JWTToken GenerateToken(IEnumerable<Claim> claims, JwtModel? jwtModel, User user, Role role)
        {
            var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtModel?.Secret ?? ""));
            var expirationTime = DateTime.UtcNow.AddDays(1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtModel?.ValidIssuer,
                Audience = jwtModel?.ValidAudience,
                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(authSignKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var jwtToken = new JWTToken
            {
                TokenString = tokenString,
                Id = user.Id,
                Name = user.FullName,
                Avatar = user.Avatar,
                Role = role.Name,
                Email = user.Email,
                ExpiresInMilliseconds = (long)(expirationTime - DateTime.UtcNow).TotalMilliseconds
            };
            return jwtToken;
        }
    }
}
