#region

using EduPlatform.Web.Pages.Basket.Dto;
using Refit;

#endregion

namespace EduPlatform.Web.Services.Refit;

public interface IDiscountRefitService
{
    [Get("/api/v1/discounts/{coupon}")]
    Task<ApiResponse<GetDiscountByCouponResponse>> GetDiscountByCoupon(string coupon);
}