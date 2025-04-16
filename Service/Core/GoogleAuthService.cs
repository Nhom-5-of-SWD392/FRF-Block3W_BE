using Data.EFCore;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Service.Utilities;

namespace Service.Core;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken);
}
public class GoogleAuthService : IGoogleAuthService
{
    private readonly DataContext _dataContext;
    private readonly IJwtUtils _jwtUtils;
    private readonly IConfiguration _configuration;

    public GoogleAuthService(DataContext dataContext, IJwtUtils jwtUtils, IConfiguration configuration)
    {
        _dataContext = dataContext;
        _jwtUtils = jwtUtils;
        _configuration = configuration;
    }
    public async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

