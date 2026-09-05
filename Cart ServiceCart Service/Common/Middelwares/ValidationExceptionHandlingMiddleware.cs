using Cart_ServiceCart_Service.Common.Enums;
using Cart_ServiceCart_Service.Common.Exceptions;
using Cart_ServiceCart_Service.Common.ResultPattern;
namespace Cart_ServiceCart_Service.Common.Middelwares;

public class ValidationExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RequestValidationException exception)
        {
            string message = exception.Message;
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var result = EndpointResponse<bool>.Failure(ErrorCode.InvalidInput, message);

            await context.Response.WriteAsJsonAsync(result);
        }
    }
}
