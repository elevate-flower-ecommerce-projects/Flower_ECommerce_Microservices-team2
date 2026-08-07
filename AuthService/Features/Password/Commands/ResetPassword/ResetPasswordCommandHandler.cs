using AuthService.Common.BaseHandler;
using AuthService.Common.ResultPattern;

namespace AuthService.Features.Password.Commands.ResetPassword
{
    /// <summary>
    /// Finds user by email, finds valid OTP, checks code + expiry, hashes new password, updates user.Password,
    /// soft-deletes the OTP. Returns RequestResult<bool>.Success(true) or throws BusinessException.
    /// </summary>
    public class ResetPasswordCommandHandler : BaseHandler<ResetPasswordCommand, RequestResult<bool>>
    {
        public ResetPasswordCommandHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override Task<RequestResult<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
