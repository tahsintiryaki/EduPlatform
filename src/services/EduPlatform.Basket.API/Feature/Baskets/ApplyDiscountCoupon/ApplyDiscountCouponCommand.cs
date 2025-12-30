using EduPlatform.Shared;

namespace EduPlatform.Basket.API.Feature.Baskets.ApplyDiscountCoupon;

public record ApplyDiscountCouponCommand(string Coupon, float DiscountRate) : IRequestByServiceResult;