using Microsoft.AspNetCore.Mvc;
using MediatR;
using AuthService.Features.Lookup.Queries;

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
}
