using AuthService.Common.ResultPattern;
using AuthService.Features.DriverRegistration.Extension;
using AuthService.Features.DriverRegistration.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Features.DriverRegistration.EndPoint
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverRegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DriverRegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("driverregister")]
        public async Task<EndpointResponse<bool>> DriverRegister([FromBody] DriverRegistrationViewModel viewModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(viewModel.ToCommand(), cancellationToken);

            if (result.IsSuccess)
                return EndpointResponse<bool>.Success(result.Data, result.Message);

            return EndpointResponse<bool>.Failure(result.ErrorCode, result.Message);
        }
    }
}
