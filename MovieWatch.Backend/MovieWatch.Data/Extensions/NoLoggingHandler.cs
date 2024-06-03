namespace MovieWatch.Data.Extensions;

public class NoLoggingHandler : DelegatingHandler
{
    public NoLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Do not log the request or response
        return await base.SendAsync(request, cancellationToken);
    }
}
