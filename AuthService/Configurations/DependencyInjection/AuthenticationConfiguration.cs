using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AuthService.Configurations.DependencyInjection
{
    public static class AuthenticationConfiguration
    {
        public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

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
                    OnTokenValidated = async context =>
                    {
                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (!long.TryParse(userId, out var parsedUserId))
                        {
                            context.Fail("The access token does not identify a user.");
                            return;
                        }

                        var db = context.HttpContext.RequestServices.GetRequiredService<AuthDbContext>();
                        var passwordChangedAt = await db.Users
                            .Where(user => user.Id == parsedUserId)
                            .Select(user => user.PasswordChangedAt)
                            .SingleOrDefaultAsync(context.HttpContext.RequestAborted);

                        if (passwordChangedAt.HasValue && context.SecurityToken.ValidFrom <= passwordChangedAt.Value)
                            context.Fail("The access token was issued before the password was changed.");
                    }
                };
            });

            return services;
        }
    }
}
