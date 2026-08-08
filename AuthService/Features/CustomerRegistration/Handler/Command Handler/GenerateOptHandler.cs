using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using AuthService.Features.CustomerRegistration.Command.Verification;

namespace AuthService.Features.CustomerRegistration.Handler.Command_Handler
{
    public class GenerateOptHandler : BaseHandler<GenerateOptCommand, RequestResult<string>>
    {
        public GenerateOptHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<string>> Handle(GenerateOptCommand request, CancellationToken cancellationToken)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var otpVerification = new OtpVerificationCode
            {
                Id         = _snowflake.CreateId(),
                UserId     = request.userId,
                GeneratedCode = otp,
                ExpireDate = DateTime.UtcNow.AddMinutes(5)
            };

            await _context.OtpVerificationCodes.AddAsync(otpVerification, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return RequestResult<string>.Success(otp, "OTP generated successfully.");
        }
    }
}
