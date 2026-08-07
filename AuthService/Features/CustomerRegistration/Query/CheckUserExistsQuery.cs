using AuthService.Common.ResultPattern;
using MediatR;

namespace AuthService.Features.CustomerRegistration.Query
{
    public record CheckUserExistsQuery(string Email, string PhoneNumber) : IRequest<RequestResult<bool>>;
}
