using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using AuthService.Features.UserManagement.LoginFeature.Command;
using AuthService.Features.UserManagement.LoginFeature.Dto;
using AuthService.Features.UserManagement.LoginFeature.Orchestrator;
using AuthService.Features.Users.UpdateProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Features.UserManagement.LoginFeature.Orchestrator.Handler
{
    public class GenerateTokenOrchestratorHandler : BaseHandler<GenerateTokenOrchestrator, RequestResult<TokenResponse>>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GenerateTokenOrchestratorHandler> _logger;

        public GenerateTokenOrchestratorHandler(
            BaseParameters baseParameters,
            IConfiguration configuration,
            ILogger<GenerateTokenOrchestratorHandler> logger)
            : base(baseParameters)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override async Task<RequestResult<TokenResponse>> Handle(GenerateTokenOrchestrator request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.DriverProfile)
                    .ThenInclude(dp => dp.Status)
                .Include(u => u.Documents)
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return RequestResult<TokenResponse>.Failure(ErrorCode.UserNotFound, "User not found.");
            }

            if (user.IsBlocked)
            {
                return RequestResult<TokenResponse>.Failure(ErrorCode.AccountBlocked, "Account is blocked.");
            }

            var isPasswordValid = PasswordHasher.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                return RequestResult<TokenResponse>.Failure(ErrorCode.InvalidCredentials, "Invalid credentials.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender
            };

            var jwtTokenResult = await _mediator.Send(new GenerateJwtTokenCommand(userDto), cancellationToken);
            if (!jwtTokenResult.IsSuccess)
            {
                return RequestResult<TokenResponse>.Failure(jwtTokenResult.ErrorCode, jwtTokenResult.Message);
            }

            var refreshTokenResult = await _mediator.Send(new GenerateRefreshTokenCommand(new User { Id = user.Id }), cancellationToken);
            if (!refreshTokenResult.IsSuccess)
            {
                return RequestResult<TokenResponse>.Failure(refreshTokenResult.ErrorCode, refreshTokenResult.Message);
            }

            var expireMinutesStr = _configuration["Jwt:ExpireMinutes"];
            var expireMinutes = double.TryParse(expireMinutesStr, out var parsedMinutes) ? parsedMinutes : 60;
            var expiresInSeconds = (int)(expireMinutes * 60);

            string? driverStatus = null;
            if (user.DriverProfile != null && user.DriverProfile.Status != null)
            {
                driverStatus = user.DriverProfile.Status.StatusType.ToString();
            }

            var primaryRole = user.UserRoles.FirstOrDefault()?.Role?.Name;
            if (string.IsNullOrEmpty(primaryRole))
            {
                primaryRole = user.DriverProfile != null ? "Driver" : "Customer";
            }

            var photoUrl = user.Documents
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => d.DocumentUrl)
                .FirstOrDefault();

            var userProfile = new UserProfileResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender.ToString(),
                Role = primaryRole,
                PhotoUrl = photoUrl,
                Status = user.IsBlocked ? "Blocked" : "Active"
            };

            return RequestResult<TokenResponse>.Success(new TokenResponse
            {
                AccessToken = jwtTokenResult.Data,
                RefreshToken = refreshTokenResult.Data,
                ExpiresIn = expiresInSeconds,
                DriverStatus = driverStatus,
                User = userProfile
            }, "Login successful.");
        }
    }
}
