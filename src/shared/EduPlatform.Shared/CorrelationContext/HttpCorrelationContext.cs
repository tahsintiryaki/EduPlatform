using Microsoft.AspNetCore.Http;

namespace EduPlatform.Shared.CorrelationContext;

public sealed class HttpCorrelationContext(IHttpContextAccessor accessor) : ICorrelationContext
{
    public Guid CorrelationId
    {
        get
        {
            var ctx = accessor.HttpContext;
            if (ctx == null) return Guid.Empty;

            if (ctx.Items.TryGetValue("X-Correlation-Id", out var v) && v is Guid g)
                return g;

            if (Guid.TryParse(ctx.Request.Headers["X-Correlation-Id"], out var parsed))
                return parsed;

            return Guid.Empty;
        }
    }
}