using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieWatch.Data.Configurations;

namespace MovieWatch.Services.Services;

public interface IJwtService
{
    Task<string?> GenerateJwtToken(int userId);
    Task<int?> GetUserIdFromToken(string token);
    
}

public class JwtService : IJwtService
{
    private readonly IOptionsMonitor<JwtSettings> _jwtSettings;
    
    public JwtService(IOptionsMonitor<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }
    

    public async Task<string?> GenerateJwtToken(int userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.CurrentValue.Secret);
        var expirationInMinutes = _jwtSettings.CurrentValue.ExpirationInMinutes;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return await Task.FromResult(tokenHandler.WriteToken(token));
    }
    
    public async Task<int?> GetUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.CurrentValue.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);
            var jwtToken = (JwtSecurityToken) validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            return await Task.FromResult(userId);
        }
        catch
        {
            return await Task.FromResult((int?) null);
        }
    }
    
}