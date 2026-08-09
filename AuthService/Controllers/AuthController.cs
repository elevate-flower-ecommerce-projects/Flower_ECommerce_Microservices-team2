using AuthService.Common.ResultPattern;
using AuthService.Features.Lookup.Queries;
using AuthService.Features.Password.Commands.ChangePassword;
using AuthService.Features.Password.Commands.ForgotPassword;
using AuthService.Features.Password.Commands.ResetPassword;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

/// <summary>
/// Handles authentication and health-check operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns the current health status of the Auth Service.
    /// </summary>
    /// <returns>200 OK with a status message.</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health() => Ok(new { status = "AuthService is running" });

    /// <summary>
    /// Test endpoint to retrieve all PersonTypes and Statuses from the database.
    /// Used for testing database connectivity and data retrieval.
    /// </summary>
    /// <returns>200 OK with PersonTypes and Statuses data.</returns>
    [HttpGet("test-db")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            var result = await _mediator.Send(new GetLookupsQuery());
            return Ok(new
            {
                success = true,
                message = "Database connection successful",
                data = result
             });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Database connection failed",
                error = ex.Message
            });
        }
    }

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

    /// <summary>
    /// Changes the password for the currently logged-in user.
    /// </summary>
    /// <param name="request">Change Password parameters.</param>
    /// <returns>Endpoint response.</returns>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var command = request.Adapt<ChangePasswordCommand>();
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(EndpointResponse<bool>.Success(result.Data, result.Message))
            : BadRequest(EndpointResponse<bool>.Failure(result.ErrorCode, result.Message));
    }


}
