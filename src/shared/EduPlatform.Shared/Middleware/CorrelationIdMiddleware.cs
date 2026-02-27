using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EduPlatform.Shared.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";

    public async Task Invoke(HttpContext context)
    {
        var cidStr = context.Request.Headers[HeaderName].FirstOrDefault();
        var cid = Guid.TryParse(cidStr, out var parsed) ? parsed : Guid.NewGuid();

        context.Items[HeaderName] = cid;
        context.Response.Headers[HeaderName] = cid.ToString();

        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = cid
               }))
        {
            await next(context);
        }
    }
}
