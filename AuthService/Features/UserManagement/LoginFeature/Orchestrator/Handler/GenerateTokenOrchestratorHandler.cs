using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Command;
using AuthService.Features.UserManagement.LoginFeature.Dto;
using AuthService.Entities;
using AuthService.Features.UserManagement.LoginFeature.Orchestrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AuthService.Features.UserManagement.LoginFeature.Queries;

namespace AuthService.Features.UserManagement.LoginFeature.Orchestrator.Handler
{
    public class GenerateTokenOrchestratorHandler : BaseHandler<GenerateTokenOrchestrator, RequestResult<TokenResponse>>
    {
        private readonly ILogger<GenerateTokenOrchestratorHandler> _logger;

        public GenerateTokenOrchestratorHandler(BaseParameters baseParameters, ILogger<GenerateTokenOrchestratorHandler> logger)
            : base(baseParameters)
        {
            _logger = logger;
        }

        public override async Task<RequestResult<TokenResponse>> Handle(GenerateTokenOrchestrator request, CancellationToken cancellationToken)
        {
                var userAuthResult = await _mediator.Send(new GetUserAuthQuery(request.Email), cancellationToken);

                if (userAuthResult.Data == null)
                { 
                    return RequestResult<TokenResponse>.Failure(ErrorCode.UserNotFound, "User not found.");
                }

                var userRecord = userAuthResult.Data;

                if (userRecord == null)
                {
                    return RequestResult<TokenResponse>.Failure(ErrorCode.UserNotFound, "User not found.");
                }

                var isPasswordValid = PasswordHasher.Verify(request.Password, userRecord.Password);
                if (!isPasswordValid)
                {
                    return RequestResult<TokenResponse>.Failure(ErrorCode.InvalidCredentials, "Invalid credentials.");
                }

                var userDto = new UserDto
                {
                    Id = userRecord.Id,
                    FullName = userRecord.FullName,
                    Email = userRecord.Email,
                    PhoneNumber = userRecord.PhoneNumber,
                    Gender = userRecord.Gender
                };

                var jwtTokenResult = await _mediator.Send(new GenerateJwtTokenCommand(userDto), cancellationToken);
                if (!jwtTokenResult.IsSuccess)
                {
                    return RequestResult<TokenResponse>.Failure(jwtTokenResult.ErrorCode, jwtTokenResult.Message);
                }

                var refreshTokenResult = await _mediator.Send(new GenerateRefreshTokenCommand(new User { Id = userRecord.Id }), cancellationToken);
                if (!refreshTokenResult.IsSuccess)
                {
                    return RequestResult<TokenResponse>.Failure(refreshTokenResult.ErrorCode, refreshTokenResult.Message);
                }

                return RequestResult<TokenResponse>.Success(new TokenResponse
                {
                    Token = jwtTokenResult.Data,
                    RefreshToken = refreshTokenResult.Data
                }, "Login successful.");
            
        }
    }
}
