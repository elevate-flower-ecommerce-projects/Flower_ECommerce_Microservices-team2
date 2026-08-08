using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Common.Services;
using AuthService.Features.CustomerRegistration.Command.Customer;
using AuthService.Features.CustomerRegistration.Command.Orchestrator;
using AuthService.Features.CustomerRegistration.Command.Verification;
using Hangfire;

namespace AuthService.Features.CustomerRegistration.Handler.Command_Handler;


public class RegisterCustomerOrchestratorHandler : BaseHandler<RegisterCustomerOrchestratorCommand, RequestResult<bool>>
{
    private readonly IBackgroundJobClient _jobClient;

    public RegisterCustomerOrchestratorHandler(
        BaseParameters baseParameters,
        IBackgroundJobClient jobClient) : base(baseParameters)
    {
        _jobClient    = jobClient;
    }

    public override async Task<RequestResult<bool>> Handle(
        RegisterCustomerOrchestratorCommand request,
        CancellationToken cancellationToken)
    {
        var registrationResult = await _mediator.Send(
            new CustomerRegistrationCommand(
                request.FullName,
                request.Email,
                request.PhoneNumber,
                request.Gender,
                request.Password,
                request.ConfirmPassword),
            cancellationToken);

        if (!registrationResult.IsSuccess)
            return RequestResult<bool>.Failure(registrationResult.ErrorCode, registrationResult.Message);

        long userId = registrationResult.Data;

        var otpResult = await _mediator.Send(
            new GenerateOptCommand(userId),
            cancellationToken);

        if (!otpResult.IsSuccess)
            return RequestResult<bool>.Failure(otpResult.ErrorCode, otpResult.Message);

        string otpCode  = otpResult.Data;
        string toEmail  = request.Email;
        string fullName = request.FullName;

        _jobClient.Enqueue<IEmailService>(svc =>
            svc.SendOtpAsync(
                toEmail,
                fullName,
                otpCode,
                CancellationToken.None));

        return RequestResult<bool>.Success(true,
            "Registration successful. Please check your email for the verification code.");
    }
}

