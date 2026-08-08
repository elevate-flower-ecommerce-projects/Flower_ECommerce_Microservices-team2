using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;

namespace AuthService.Features.CustomerRegistration.Command.Verification
{
    public record GenerateOptCommand(long userId) : ICommand<RequestResult<string>>;
 
}
