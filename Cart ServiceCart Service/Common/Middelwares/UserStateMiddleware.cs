using Cart_ServiceCart_Service.Common.Services;
namespace Cart_ServiceCart_Service.Common.Middelwares;
/// <summary>
/// Fills the scoped UserState from the claims on the current request.
/// Must run after UseAuthentication() — HttpContext.User is not populated before that.
///
/// This is the only type in the service that depends on <see cref="IUserStateWriter"/>;
/// everything else takes the read-only <see cref="IUserState"/>.
/// </summary>
public class UserStateMiddleware : IMiddleware
{
    private readonly IUserStateWriter _userStateWriter;

    public UserStateMiddleware(IUserStateWriter userStateWriter)
    {
        _userStateWriter = userStateWriter;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Same scope as the request, so this is the instance every handler will resolve.
        _userStateWriter.Populate(context.User);

        await next(context);
    }
}
