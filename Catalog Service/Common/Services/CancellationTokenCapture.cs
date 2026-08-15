namespace Catalog_Service.Common.Services
{
    /// <summary>
    /// Per-request holder for the request's <see cref="CancellationToken"/>, captured once by
    /// <see cref="Middelwares.CancellationTokenCaptureMiddleware"/> from HttpContext.RequestAborted.
    /// Registered scoped, so anything resolved during the request (handlers, services) can honour
    /// client disconnects without having the token threaded through every call signature.
    ///
    /// Defaults to <see cref="CancellationToken.None"/> outside a request (background/CAP consumers,
    /// design-time), which means "never cancels" rather than throwing.
    /// </summary>
    public class CancellationTokenCapture
    {
        public CancellationToken Token { get; private set; } = CancellationToken.None;

        public bool IsCancellationRequested => Token.IsCancellationRequested;

        /// <summary>Throws <see cref="OperationCanceledException"/> if the client has gone away.</summary>
        public void ThrowIfCancellationRequested() => Token.ThrowIfCancellationRequested();

        /// <summary>Called by the middleware only — one capture per request.</summary>
        public void Capture(CancellationToken token) => Token = token;
    }
}
