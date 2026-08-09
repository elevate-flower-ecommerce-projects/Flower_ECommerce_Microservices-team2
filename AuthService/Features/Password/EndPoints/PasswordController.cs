using AuthService.Common.ResultPattern;
using AuthService.Features.Password.Commands.ForgotPassword;
using AuthService.Features.Password.Commands.ResetPassword;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
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
        var command = request.Adapt<ForgotPasswordCommand>();
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(EndpointResponse<bool>.Success(result.Data, result.Message))
            : BadRequest(EndpointResponse<bool>.Failure(result.ErrorCode, result.Message));
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
        var command = request.Adapt<ResetPasswordCommand>();
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(EndpointResponse<bool>.Success(result.Data, result.Message))
            : BadRequest(EndpointResponse<bool>.Failure(result.ErrorCode, result.Message));
    }


}
