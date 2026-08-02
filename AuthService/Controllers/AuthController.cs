using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

/// <summary>
/// Handles authentication and health-check operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Returns the current health status of the Auth Service.
    /// </summary>
    /// <returns>200 OK with a status message.</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health() => Ok(new { status = "AuthService is running" });
}
