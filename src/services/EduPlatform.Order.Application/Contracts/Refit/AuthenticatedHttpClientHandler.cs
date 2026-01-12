#region

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

#endregion

namespace EduPlatform.Order.Application.Contracts.Refit;

internal class AuthenticatedHttpClientHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        //İstek order üzerinde giderse HttpContext dolu olur fakat istek background üzerindneb başlarsa context null olur.
        if (httpContextAccessor.HttpContext is null) return await base.SendAsync(request, cancellationToken);

        if (!httpContextAccessor.HttpContext!.User.Identity!.IsAuthenticated)
            return await base.SendAsync(request, cancellationToken);


        string? token = null;

        if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            // Bearer abc
            token = authHeader.ToString().Split(" ")[1];


        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


        return await base.SendAsync(request, cancellationToken);
    }
}