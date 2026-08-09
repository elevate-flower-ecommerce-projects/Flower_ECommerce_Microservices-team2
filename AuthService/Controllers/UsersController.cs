using AuthService.Common.ResultPattern;
using AuthService.Features.Users.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPut("me")]
    [ProducesResponseType(typeof(EndpointResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<UserProfileResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EndpointResponse<UserProfileResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EndpointResponse<UserProfileResponse>>> UpdateProfile(
        [FromBody] UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(
                EndpointResponse<UserProfileResponse>.Failure(result.ErrorCode, result.Message));
        }

        return Ok(EndpointResponse<UserProfileResponse>.Success(result.Data, result.Message));
    }
}