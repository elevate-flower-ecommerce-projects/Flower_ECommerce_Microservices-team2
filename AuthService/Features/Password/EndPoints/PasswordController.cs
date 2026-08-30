using AuthService.Common.ResultPattern;
using AuthService.Features.Password.Commands.ForgotPassword;
using AuthService.Features.Password.Commands.ResetPassword;
using AuthService.Features.Password.Commands.VerifyOtp;
using AuthService.Features.Password.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Features.Password.EndPoints;

/// <summary>
/// Handles Forgot/Reset Password.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PasswordController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Initiates the forgot password flow by generating and sending an OTP verification code.
    /// </summary>
    /// <param name="request">Forgot Password request containing user email.</param>
    /// <returns>Endpoint response.</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _mediator.Send(new ForgotPasswordCommand(request.Email));

        return result.IsSuccess
            ? Ok(EndpointResponse<bool>.Success(result.Data, result.Message))
            : BadRequest(EndpointResponse<bool>.Failure(result.ErrorCode, result.Message));
    }

    /// <summary>
    /// Verifies the 6-digit OTP code and returns a short-lived reset authorization token.
    /// </summary>
    /// <param name="request">Verify OTP request.</param>
    /// <returns>Endpoint response with reset authorization token.</returns>
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(EndpointResponse<VerifyOtpResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<VerifyOtpResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyOtpRequest request)
    {
        var result = await _mediator.Send(new VerifyOtpCommand(request.Email, request.Otp));

        return result.IsSuccess
            ? Ok(EndpointResponse<VerifyOtpResponse>.Success(result.Data, result.Message))
            : BadRequest(EndpointResponse<VerifyOtpResponse>.Failure(result.ErrorCode, result.Message));
    }

    /// <summary>
    /// Resets the user's password using an OTP verification code.
    /// </summary>
    /// <param name="request">Reset Password parameters.</param>
    /// <returns>Endpoint response.</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _mediator.Send(new ResetPasswordCommand(request.Email, request.ResetCode, request.NewPassword));

        return result.IsSuccess
            ? Ok(EndpointResponse<bool>.Success(result.Data, result.Message))
            : BadRequest(EndpointResponse<bool>.Failure(result.ErrorCode, result.Message));
    }
}
