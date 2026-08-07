using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using AuthService.Features.Common.Role.Query;
using AuthService.Features.CustomerRegistration.Command;
using AuthService.Features.CustomerRegistration.Query;
using IdGen;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.CustomerRegistration.Handler
{
    public class CustomerRegistrationHandler : BaseHandler<CustomerRegistrationCommand, RequestResult<bool>>
    {
        public CustomerRegistrationHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<bool>> Handle(CustomerRegistrationCommand request, CancellationToken cancellationToken)
        {
            var userExistsResult = await _mediator.Send(new CheckUserExistsQuery(request.Email, request.PhoneNumber), cancellationToken);
            if (userExistsResult.Data)
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, "User with this email or phone number already exists.");

            var customerRoleResult = await _mediator.Send(new GetRoleByEnumQuery(PersonTypeEnum.Customer), cancellationToken);
            if (!customerRoleResult.IsSuccess)
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, customerRoleResult.Message);

            var user = new User();
            user.Id = _snowflake.CreateId();
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Gender = request.Gender;
            user.Password = PasswordHasher.Hash(request.Password);

            _context.Users.Add(user);
            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = customerRoleResult.Data });

            await _context.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true, "Registration has been completed successfully");
        }
    }
}
