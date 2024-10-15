namespace ClockApi.Auth;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RoleAuthorizationHandler> _logger;
    private readonly AuthContext _dbContext;

    public RoleAuthorizationHandler(IHttpContextAccessor httpContextAccessor, ILogger<RoleAuthorizationHandler> logger, AuthContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", string.Empty).Trim();

        if (token != null)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("JwtKey"))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                var userIdClaim = principal.FindFirst("custom_user_id")?.Value;
                if (userIdClaim != null)
                {
                    _logger.LogInformation("userID: {userId}", userIdClaim);

                    var user = await _dbContext.Users.FindAsync(int.Parse(userIdClaim));
                    if (user == null)
                    {
                        _logger.LogWarning($"User with ID {userIdClaim} not found in database.");
                        context.Fail();
                        return;
                    }

                    if (!user.Roles.Split(',').Contains(requirement.Role))
                    {
                        _logger.LogWarning($"User with ID {userIdClaim} does not have the required role: {requirement.Role}");
                        context.Fail();
                        return;
                    }

                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogWarning("User ID claim not found in token.");
                    context.Fail();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token.");
                context.Fail();
            }
        }
        else
        {
            _logger.LogWarning("Authorization header not found in request.");
            context.Fail();
        }
    }
}