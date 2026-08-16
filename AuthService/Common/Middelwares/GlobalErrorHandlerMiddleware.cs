using AuthService.Common.Enums;
using AuthService.Common.Exceptions;
using AuthService.Common.ResultPattern;
using MediatR;

namespace AuthService.Common.Middelwares
{
    public class GlobalErrorHandlerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException)
            {
                var result = EndpointResponse<bool>.Failure(ErrorCode.ClientClosedRequest);
                await context.Response.WriteAsJsonAsync(result);
            }
            catch (BusinessException ex)
            {
                var result = EndpointResponse<bool>.Failure(ex.ErrorCode, ex.Message);
                await context.Response.WriteAsJsonAsync(result);
            }
            catch (Exception ex)
            {
                string message = $"Error Occured: {ex.Message}.";
                ErrorCode errorCode = ErrorCode.UnKnown;

                var result = EndpointResponse<bool>.Failure(errorCode, message);
                await context.Response.WriteAsJsonAsync(result);
            }
        }
    }
}
