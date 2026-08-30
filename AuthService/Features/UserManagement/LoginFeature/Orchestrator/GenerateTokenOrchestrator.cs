using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Dto;

namespace AuthService.Features.UserManagement.LoginFeature.Orchestrator
{
    public record GenerateTokenOrchestrator(
        string Email,
        string Password,
        string? FcmToken = null,
        string? DeviceId = null) : ICommand<RequestResult<TokenResponse>>;
}
