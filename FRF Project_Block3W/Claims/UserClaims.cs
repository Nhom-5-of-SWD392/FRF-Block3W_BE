using System.Security.Claims;

namespace FRF_Project_Block3W.Claims;

public static class UserClaims
{
    public static string GetUserNameFromJwtToken(this IEnumerable<Claim> claims)
    {
        var userName = claims.FirstOrDefault(claims => claims.Type == ClaimTypes.Name)?.Value;
        return userName ?? "";
    }

    public static string GetUserIdFromJwtToken(this IEnumerable<Claim> claims)
    {
        var userId = claims.FirstOrDefault(claims => claims.Type == "id")?.Value;
        return userId ?? "";
    }

    public static string GetUserRoleFromJwtToken(this IEnumerable<Claim> claims)
    {
        var userId = claims.FirstOrDefault(claims => claims.Type == ClaimTypes.Role)?.Value;
        return userId ?? "";
    }
}
