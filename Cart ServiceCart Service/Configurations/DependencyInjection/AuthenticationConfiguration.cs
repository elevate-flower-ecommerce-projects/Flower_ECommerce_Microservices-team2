using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
namespace Cart_ServiceCart_Service.Configurations.DependencyInjection;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");

        var secretKey = jwtSettings["Key"];
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException(
                "Missing 'Jwt:JWT_SECRET' configuration. It must match the key AuthService signs tokens with " +
                "(supply it via the JWT_SECRET environment variable or user-secrets).");
        }

        // UTF8 — must match the encoding AuthService uses when signing (GenerateJwtTokenHandler).
        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(key),
                ValidateIssuer           = true,
                ValidIssuer              = jwtSettings["Issuer"],
                ValidateAudience         = true,
                ValidAudience            = jwtSettings["Audience"],
                ValidateLifetime         = true,
                ClockSkew                = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    // Catalog Service is a resource server with no user store, so it cannot repeat
                    // AuthService's "issued before the password changed" check. What it can enforce
                    // is that the token carries a usable user id — CurrentUserService parses this
                    // claim as a long, and a malformed value would fault the request later.
                    var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!long.TryParse(userId, out _))
                        context.Fail("The access token does not identify a user.");

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}
