using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Command;
using AuthService.Features.UserManagement.LoginFeature.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Features.UserManagement.LoginFeature.Command.Handler
{
    public class GenerateJwtTokenHandler : BaseHandler<GenerateJwtTokenCommand, RequestResult<string>>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GenerateJwtTokenHandler> _logger;

        public GenerateJwtTokenHandler(BaseParameters baseParameters, IConfiguration configuration, ILogger<GenerateJwtTokenHandler> logger)
            : base(baseParameters)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override async Task<RequestResult<string>> Handle(GenerateJwtTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var secretKey = _configuration["Jwt:Key"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, request.User.FullName ?? string.Empty),
                    new Claim(ClaimTypes.Email, request.User.Email ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, request.User.Id.ToString())
                };

                var rolesResult = await _mediator.Send(new GetUserRolesQuery(request.User.Id), cancellationToken);

                if (rolesResult.IsSuccess && rolesResult.Data != null)
                {
                    foreach (var roleName in rolesResult.Data)
                    {
                        if (!string.IsNullOrEmpty(roleName))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, roleName));
                        }
                    }
                }

                var expireMinutesStr = _configuration["Jwt:ExpireMinutes"];
                var expireMinutes = double.TryParse(expireMinutesStr, out var parsedMinutes) ? parsedMinutes : 60;

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"] ?? "AuthService",
                    audience: _configuration["Jwt:Audience"] ?? "AuthServiceClients",
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return RequestResult<string>.Success(tokenString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating JWT token for user {UserId}", request.User.Id);
                return RequestResult<string>.Failure(ErrorCode.FailWhileGenerateToken, "An error occurred while generating JWT token.");
            }
        }
    }
}
