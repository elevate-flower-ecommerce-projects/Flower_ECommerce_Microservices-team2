using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Features.CustomerRegistration.EndPoint
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationEmailController : ControllerBase
    {





        [HttpGet]
        public IActionResult VerifyEmail(string token)
        {
            // Here you would typically call a service to verify the token and activate the user's account.
            // For demonstration purposes, we'll just return a success message.
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token.");
            }
            // Simulate token verification logic
            bool isTokenValid = true; // Replace with actual verification logic
            if (isTokenValid)
            {
                return Ok("Email verified successfully.");
            }
            else
            {
                return BadRequest("Invalid or expired token.");
            }
        }
    }
}
