using AuthService.Common.ResultPattern;
using AuthService.Features.CustomerRegistration.Extension;
using AuthService.Features.CustomerRegistration.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Features.CustomerRegistration.EndPoint
{
    [Route("api/CustomerRegistration/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerRegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<EndpointResponse<bool>> Register([FromBody] CustomerRegistrationViewModel viewModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(viewModel.ToCommand(), cancellationToken);

            if (result.IsSuccess)
                return EndpointResponse<bool>.Success(result.Data, result.Message);
            else
                return EndpointResponse<bool>.Failure(result.ErrorCode, result.Message);
        }
    }
}
