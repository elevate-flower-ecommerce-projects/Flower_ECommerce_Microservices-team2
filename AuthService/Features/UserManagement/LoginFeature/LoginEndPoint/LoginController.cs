using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Command;
using AuthService.Features.UserManagement.LoginFeature.Dto;
using AuthService.Features.UserManagement.LoginFeature.Extension;
using AuthService.Features.UserManagement.LoginFeature.Orchestrator;
using AuthService.Features.UserManagement.LoginFeature.Queries;
using AuthService.Features.UserManagement.LoginFeature.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Features.UserManagement.LoginFeature.LoginEndPoint
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<EndpointResponse<TokenResponse>> Login([FromBody] LoginRequestVm request, CancellationToken ct)
        {
            var requestResult = await _mediator.Send(request.ToCommand(), ct);

            if (!requestResult.IsSuccess)
                return EndpointResponse<TokenResponse>.Failure(requestResult.ErrorCode, requestResult.Message);

            return EndpointResponse<TokenResponse>.Success(requestResult.Data, requestResult.Message);
        }

        
        [HttpPost("reactivate")]
        public async Task<EndpointResponse<string>> ReActivateToken([FromBody] long userId, CancellationToken ct)
        {
          
            var userRefreshToken = await _mediator.Send(new GetActiveRefreshTokenQuery(userId), ct);

            if (userRefreshToken == null)
                return EndpointResponse<string>.Failure(ErrorCode.InvalidToken, "Refresh token not found.");

            if (userRefreshToken.ExpireDate < DateTime.UtcNow)
                return EndpointResponse<string>.Failure(ErrorCode.InvalidToken, "Refresh token has expired, please login again.");

            var userResult = await _mediator.Send(new GetUserByIdQuery(userId), ct);

            if (!userResult.IsSuccess || userResult.Data == null)
                return EndpointResponse<string>.Failure(ErrorCode.UserNotFound, "User not found.");

            var requestResult = await _mediator.Send(new GenerateJwtTokenCommand(userResult.Data), ct);

            if (!requestResult.IsSuccess)
                return EndpointResponse<string>.Failure(ErrorCode.InternalServerError, "Failed to generate token.");

            return EndpointResponse<string>.Success(requestResult.Data, "Token reactivated successfully.");
        }       
        [HttpPost("t")]

        [Authorize]
        public async Task<EndpointResponse<string>> t([FromBody] long userId, CancellationToken ct)
        {

            
            return EndpointResponse<string>.Success("", "Token reactivated successfully.");
        }
    }
}
