using Cart_ServiceCart_Service.Common.Services;
namespace Cart_ServiceCart_Service.Common.Middelwares;
/// <summary>
/// Captures HttpContext.RequestAborted into the scoped <see cref="CancellationTokenCapture"/>
/// so downstream code can observe client disconnects. Registered early in the pipeline —
/// the token is available from the start of the request, and capturing it before the error
/// handlers means a cancellation raised anywhere below is still surfaced correctly.
/// </summary>
public class CancellationTokenCaptureMiddleware : IMiddleware
{
    private readonly CancellationTokenCapture _cancellationTokenCapture;

    public CancellationTokenCaptureMiddleware(CancellationTokenCapture cancellationTokenCapture)
    {
        _cancellationTokenCapture = cancellationTokenCapture;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Same scope as the request, so this is the instance every handler will resolve.
        _cancellationTokenCapture.Capture(context.RequestAborted);

        await next(context);
    }
}
