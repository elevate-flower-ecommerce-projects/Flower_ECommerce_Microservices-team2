using AuthService.Common.Enums;
using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;

namespace AuthService.Features.CustomerRegistration.Command.Orchestrator;


public record RegisterCustomerOrchestratorCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    Gender Gender,
    string Password,
    string ConfirmPassword
) : ICommand<RequestResult<bool>>;
