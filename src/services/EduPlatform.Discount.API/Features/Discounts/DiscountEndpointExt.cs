using Asp.Versioning.Builder;
using EduPlatform.Discount.API.Features.Discounts.CreateDiscount;
using EduPlatform.Discount.API.Features.Discounts.GetDiscountByCode;

namespace EduPlatform.Discount.API.Features.Discounts;

public static class DiscountEndpointExt
{
    public static void AddDiscountGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        app.MapGroup("api/v{version:apiVersion}/discounts").WithTags("discounts").WithApiVersionSet(apiVersionSet)
            .CreateDiscountGroupItemEndpoint()
            .GetDiscountByCodeGroupItemEndpoint().RequireAuthorization();
    }
}