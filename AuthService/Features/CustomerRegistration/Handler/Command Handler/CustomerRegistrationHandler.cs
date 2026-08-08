using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using AuthService.Features.Common.Role.Query;
using AuthService.Features.CustomerRegistration.Command.Customer;
using AuthService.Features.CustomerRegistration.Query;

namespace AuthService.Features.CustomerRegistration.Handler.CustomerHandler
{
    public class CustomerRegistrationHandler : BaseHandler<CustomerRegistrationCommand, RequestResult<long>>
    {
        public CustomerRegistrationHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<long>> Handle(CustomerRegistrationCommand request, CancellationToken cancellationToken)
        {
            var userExistsResult = await _mediator.Send(new CheckUserExistsQuery(request.Email, request.PhoneNumber), cancellationToken);
            if (userExistsResult.Data)
                return RequestResult<long>.Failure(ErrorCode.BadRequest, "User with this email or phone number already exists.");

            var customerRoleResult = await _mediator.Send(new GetRoleByEnumQuery(PersonTypeEnum.Customer), cancellationToken);
            if (!customerRoleResult.IsSuccess)
                return RequestResult<long>.Failure(ErrorCode.BadRequest, customerRoleResult.Message);

            var user = new User();
            user.Id = _snowflake.CreateId();
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Gender = request.Gender;
            user.Password = PasswordHasher.Hash(request.Password);
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = customerRoleResult.Data }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return RequestResult<long>.Success(user.Id, "Registration has been completed successfully");
        }
    }
}
