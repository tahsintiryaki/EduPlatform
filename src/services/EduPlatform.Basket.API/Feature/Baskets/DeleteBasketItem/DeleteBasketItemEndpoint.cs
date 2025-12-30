using EduPlatform.Basket.API.Feature.Baskets.AddBasketItem;
using EduPlatform.Shared.Extensions;
using EduPlatform.Shared.Filters;
using MediatR;

namespace EduPlatform.Basket.API.Feature.Baskets.DeleteBasketItem;

public static class DeleteBasketItemEndpoint
{
    public static RouteGroupBuilder DeleteBasketItemGroupItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/item/{id:guid}",
                async (Guid id, IMediator mediator) =>
                    (await mediator.Send(new DeleteBasketItemCommand(id))).ToGenericResult())
            .WithName("DeleteBasketItem")
            .MapToApiVersion(1, 0);


        return group;
    }
}