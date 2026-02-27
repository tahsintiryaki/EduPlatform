namespace EduPlatform.Web.DelegateHandlers;

public class CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    public const string HeaderName = "X-Correlation-Id";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Web request’i sırasında üretip Items’a koyduysan buradan alırsın
        if (httpContextAccessor.HttpContext?.Items.TryGetValue(HeaderName, out var v) == true
            && v is Guid cid)
        {
            request.Headers.TryAddWithoutValidation(HeaderName, cid.ToString());
        }

        return base.SendAsync(request, cancellationToken);
    }
}