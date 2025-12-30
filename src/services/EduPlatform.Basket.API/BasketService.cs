using System.Text.Json;
using EduPlatform.Basket.API.Const;
using EduPlatform.Shared.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace EduPlatform.Basket.API;

public class BasketService(IIdentityService identityService, IDistributedCache distributedCache)
{
    private string GetCacheKey()
    {
        return string.Format(BasketConst.BasketCacheKey, identityService.UserId);
    }

    private string GetCacheKey(Guid userId)
    {
        return string.Format(BasketConst.BasketCacheKey, userId);
    }

    public Task<string?> GetBasketFromCache(CancellationToken cancellationToken)
    {
        return distributedCache.GetStringAsync(GetCacheKey(), cancellationToken);
    }

    public async Task CreateBasketCacheAsync(Data.Basket basket, CancellationToken cancellationToken)
    {
        var basketAsString = JsonSerializer.Serialize(basket);
        await distributedCache.SetStringAsync(GetCacheKey(), basketAsString, cancellationToken);
    }

    public async Task DeleteBasket(Guid userId)
    {
        await distributedCache.RemoveAsync(GetCacheKey(userId));
    }
}