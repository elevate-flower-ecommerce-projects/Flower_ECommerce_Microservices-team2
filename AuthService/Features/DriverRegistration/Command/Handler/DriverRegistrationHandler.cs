using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using AuthService.Features.Common.Role.Query;
using AuthService.Features.CustomerRegistration.Query;
using AuthService.Features.DriverRegistration.Command;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.DriverRegistration.Command.Handler
{
    public class DriverRegistrationHandler : BaseHandler<DriverRegistrationCommand, RequestResult<bool>>
    {
        public DriverRegistrationHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<bool>> Handle(DriverRegistrationCommand request, CancellationToken cancellationToken)
        {
            var userExistsResult = await _mediator.Send(new CheckUserExistsQuery(request.Email, request.PhoneNumber), cancellationToken);
            if (userExistsResult.Data)
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, "User with this email or phone number already exists.");

            var nationalIdExists = await _context.DriverUsers
                .AnyAsync(driver => driver.NationalId == request.NationalId, cancellationToken);
            if (nationalIdExists)
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, "Driver with this national ID already exists.");

            var driverRoleResult = await _mediator.Send(new GetRoleByEnumQuery(PersonTypeEnum.Driver), cancellationToken);
            if (!driverRoleResult.IsSuccess)
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, driverRoleResult.Message);

            var user = new User
            {
                Id = _snowflake.CreateId(),
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Gender = request.Gender,
                Password = PasswordHasher.Hash(request.Password)
            };

            var driver = new DriverUser
            {
                Id = _snowflake.CreateId(),
                UserId = user.Id,
                NationalId = request.NationalId,
                VehicleType = request.VehicleType,
                VehiclePlate = request.VehiclePlate,
                StatusId = (long)DriverStatus.Pending
            };

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = driverRoleResult.Data }, cancellationToken);
            await _context.DriverUsers.AddAsync(driver, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true, "Driver registration has been submitted successfully");
        }
    }
}
