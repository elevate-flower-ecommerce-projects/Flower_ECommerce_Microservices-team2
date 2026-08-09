using AuthService.Common.ResultPattern;
using AuthService.Features.Users.ChangePassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public sealed class ChangePasswordController(IMediator mediator) : ControllerBase
{
    [HttpPost("me/change-password")]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EndpointResponse<bool>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EndpointResponse<bool>>> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(
                EndpointResponse<bool>.Failure(result.ErrorCode, result.Message));
        }

        return Ok(EndpointResponse<bool>.Success(result.Data, result.Message));
    }
}