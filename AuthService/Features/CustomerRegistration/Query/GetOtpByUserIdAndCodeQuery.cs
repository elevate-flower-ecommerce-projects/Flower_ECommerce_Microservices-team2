using AuthService.Common.ResultPattern;
using AuthService.Features.CustomerRegistration.Dto;
using MediatR;

namespace AuthService.Features.CustomerRegistration.Query
{
    public record GetOtpByUserIdAndCodeQuery(long UserId, string Code) : IRequest<RequestResult<OtpVerificationDto?>>;
}
