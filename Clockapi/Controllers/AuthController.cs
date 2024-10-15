using clockapi.Auth;

namespace ClockApi.Controllers;

using Models;
using Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using clockapi.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly AuthenticationService _authenticationService;

    public AuthController(AuthContext dbContext, IConfiguration configuration, ILogger<AuthController> logger, AuthenticationService authenticationService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
        _authenticationService = authenticationService;
    }

    [Authorize]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var salt = GenerateSalt();
        var passwordHash = HashPasswordWithSalt(registerDto.Password, salt);

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = passwordHash,
            Salt = salt,
            Roles = "Admin", 
            Email = registerDto.Email 
        };

        _dbContext.Users.Add(user); // Fixed _context to _dbContext
        await _dbContext.SaveChangesAsync(); // Fixed _context to _dbContext

        return Ok();
    }

    private string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    private string HashPasswordWithSalt(string password, string salt)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (salt == null) throw new ArgumentNullException(nameof(salt));

        var sha256 = SHA256.Create();
        var saltedPassword = password + salt;
        var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
        var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
        return Convert.ToBase64String(hashBytes);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        try
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == loginModel.UserName);

            if (user == null || user.PasswordHash != HashPasswordWithSalt(loginModel.Password, user.Salt))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate JWT token
            var token = _authenticationService.GenerateJwtToken(user);

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the login request.");
            return StatusCode(500, "Internal server error");
        }
    }
}
