using System.Net;
using System.Text.Json;
using EduPlatform.Basket.API.Dtos;
using EduPlatform.Shared;
using EduPlatform.Shared.Extensions;
using EduPlatform.Shared.Filters;
using MediatR;

namespace EduPlatform.Basket.API.Feature.Baskets.ApplyDiscountCoupon;

public static class ApplyDiscountCouponEndpoint
{
    public static RouteGroupBuilder ApplyDiscountCouponGroupItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/apply-discount-coupon",
                async (ApplyDiscountCouponCommand command, IMediator mediator) =>
                    (await mediator.Send(command)).ToGenericResult())
            .WithName("ApplyDiscountCoupon")
            .MapToApiVersion(1, 0)
            .AddEndpointFilter<ValidationFilter<ApplyDiscountCouponCommand>>();
        return group;
    }
}