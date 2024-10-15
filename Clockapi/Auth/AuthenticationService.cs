using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClockApi.Models;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;

namespace clockapi.Auth;

public class AuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(ILogger<AuthenticationService> logger)
    {
        _logger = logger;
        // Load environment variables
        Env.Load("clockapi.env");
    }
    
    public string GenerateJwtToken(User user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Env.GetString("JwtKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("custom_user_id", user.Id.ToString()), // Custom user ID claim
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Roles) // Ensure roles claim
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = Env.GetString("JwtIssuer"),
                Audience = Env.GetString("JwtAudience"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user {Username}", user.Username);
            throw;
        }
    }
    
    // Additional methods for user authentication, validation, etc.
}