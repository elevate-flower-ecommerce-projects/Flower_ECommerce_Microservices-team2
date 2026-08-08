using AuthService.Common.ResultPattern;
using AuthService.Features.CustomerRegistration.Command;
using AuthService.Features.CustomerRegistration.Extension;
using AuthService.Features.CustomerRegistration.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Features.CustomerRegistration.EndPoint
{
    [Route("api/CustomerRegistration/[controller]")]
    [ApiController]
    public class CustomerRegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerRegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registers a customer (no OTP/email step).
        /// </summary>
        [HttpPost("register")]
        public async Task<EndpointResponse<bool>> Register([FromBody] CustomerRegistrationViewModel viewModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(viewModel.ToCommand(), cancellationToken);

            if (result.IsSuccess)
                return EndpointResponse<bool>.Success(true, result.Message);

            return EndpointResponse<bool>.Failure(result.ErrorCode, result.Message);
        }

        /// <summary>
        /// Full registration flow: creates user → generates OTP (5-min expiry) →
        /// sends verification email in the background via Hangfire + MailKit.
        /// </summary>
        [HttpPost("register-with-otp")]
        public async Task<EndpointResponse<bool>> RegisterWithOtp([FromBody] CustomerRegistrationViewModel viewModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(viewModel.ToOrchestratorCommand(), cancellationToken);

            if (result.IsSuccess)
                return EndpointResponse<bool>.Success(result.Data, result.Message);

            return EndpointResponse<bool>.Failure(result.ErrorCode, result.Message);
        }
    }
}
