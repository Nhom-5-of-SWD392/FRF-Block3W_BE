using Data.EFCore;
using Data.Models;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Service.Utilities;
using System.Text.Json;

namespace Service.Core;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken);
    Task<TokenResponse> ExchangeCodeForTokenAsync(string redirectUri, string code);

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
                Audience = new List<string>() { _configuration["Authentication:Google:ClientId"]! }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return payload;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string redirectUri, string code)
    {
        var clientId = _configuration["Authentication:Google:ClientId"];
        var clientSecret = _configuration["Authentication:Google:ClientSecret"];

        var values = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", clientId! },
            { "client_secret", clientSecret! },
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" }
        };

        using var client = new HttpClient();
        var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(values));

        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Raw Google response: {responseContent}"); // Debug

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Google token exchange failed. Status: {response.StatusCode}. Response: {responseContent}");
        }

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, options);

            if (tokenResponse == null)
            {
                throw new Exception("Failed to deserialize token response");
            }

            Console.WriteLine($"Deserialized token: {JsonSerializer.Serialize(tokenResponse)}");
            return tokenResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"JSON deserialization error: {ex.Message}");
        }
    }
}

