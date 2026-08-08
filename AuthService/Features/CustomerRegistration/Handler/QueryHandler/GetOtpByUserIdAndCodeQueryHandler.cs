using AuthService.Common.BaseHandler;
using AuthService.Common.ResultPattern;
using AuthService.Features.CustomerRegistration.Dto;
using AuthService.Features.CustomerRegistration.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.CustomerRegistration.Handler.QueryHandler
{
    public class GetOtpByUserIdAndCodeQueryHandler : BaseHandler<GetOtpByUserIdAndCodeQuery, RequestResult<OtpVerificationDto?>>
    {
        public GetOtpByUserIdAndCodeQueryHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<OtpVerificationDto?>> Handle(GetOtpByUserIdAndCodeQuery request, CancellationToken cancellationToken)
        {
            var otpRecord = await _context.OtpVerificationCodes
                .Where(o => o.UserId == request.UserId && o.GeneratedCode == request.Code)
                .Select(o => new OtpVerificationDto
                {
                    Id = o.Id,
                    GeneratedCode = o.GeneratedCode,
                    ExpireDate = o.ExpireDate
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (otpRecord == null)
                return RequestResult<OtpVerificationDto?>.Success(null);

            return RequestResult<OtpVerificationDto?>.Success(otpRecord);
        }
    }
}
