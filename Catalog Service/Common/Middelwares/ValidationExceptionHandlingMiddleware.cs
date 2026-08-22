using Catalog_Service.Common.Enums;
using Catalog_Service.Common.Exceptions;
using Catalog_Service.Common.ResultPattern;

namespace Catalog_Service.Common.Middelwares
{
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
                var result = EndpointResponse<bool>.Failure(ErrorCode.InvalidInput, message);

                await context.Response.WriteAsJsonAsync(result);
            }
        }
    }
}
