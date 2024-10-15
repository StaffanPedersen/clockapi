namespace ClockApi.Auth;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

public static class JwtConfig
{
    public static void AddJwtAuthentication(this IServiceCollection services)
    {
        
        var jwtKey = Env.GetString("JwtKey");
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new ArgumentException("Environment variable 'JwtKey' is null or empty.");
        }
        var key = Convert.FromBase64String(jwtKey);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Env.GetString("JwtIssuer"),
                    ValidAudience = Env.GetString("JwtAudience"),
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
}