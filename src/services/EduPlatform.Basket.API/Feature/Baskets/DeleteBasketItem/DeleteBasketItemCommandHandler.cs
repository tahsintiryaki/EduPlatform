using System.Net;
using System.Text.Json;
using EduPlatform.Basket.API.Const;
using EduPlatform.Basket.API.Dtos;
using EduPlatform.Shared;
using EduPlatform.Shared.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace EduPlatform.Basket.API.Feature.Baskets.DeleteBasketItem;

public class DeleteBasketItemCommandHandler(
    BasketService basketService)
    : IRequestHandler<DeleteBasketItemCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(DeleteBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basketAsJson = await basketService.GetBasketFromCache(cancellationToken);

        if (string.IsNullOrEmpty(basketAsJson)) return ServiceResult.Error("Basket not found", HttpStatusCode.NotFound);

        var currentBasket = JsonSerializer.Deserialize<Data.Basket>(basketAsJson);

        var basketItemToDelete = currentBasket!.Items.FirstOrDefault(x => x.Id == request.Id);

        if (basketItemToDelete is null) return ServiceResult.Error("Basket item not found", HttpStatusCode.NotFound);

        currentBasket.Items.Remove(basketItemToDelete);

        basketAsJson = JsonSerializer.Serialize(currentBasket);

        await basketService.CreateBasketCacheAsync(currentBasket, cancellationToken);


        return ServiceResult.SuccessAsNoContent();
    }
}