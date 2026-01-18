using Asp.Versioning.Builder;
using EduPlatform.Basket.API.Feature.Baskets.AddBasketItem;
using EduPlatform.Basket.API.Feature.Baskets.ApplyDiscountCoupon;
using EduPlatform.Basket.API.Feature.Baskets.DeleteBasketItem;
using EduPlatform.Basket.API.Feature.Baskets.GetBasket;
using EduPlatform.Basket.API.Feature.Baskets.RemoveDiscountCoupon;

namespace EduPlatform.Basket.API.Feature.Baskets;

public static class BasketEndpointExt
{
    public static void AddBasketGroupEndpointExt(this WebApplication app,ApiVersionSet apiVersionSet)
    {
        app.MapGroup("api/v{version:apiVersion}/baskets").WithTags("Baskets")
            .WithApiVersionSet(apiVersionSet)
            .AddBasketItemGroupItemEndpoint()
            .DeleteBasketItemGroupItemEndpoint()
            .GetBasketGroupItemEndpoint()
            .ApplyDiscountCouponGroupItemEndpoint()
            .RemoveDiscountCouponGroupItemEndpoint().RequireAuthorization("Password");
    }
}