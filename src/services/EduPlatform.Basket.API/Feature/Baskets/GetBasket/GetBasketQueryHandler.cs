using System.Net;
using System.Text.Json;
using AutoMapper;
using EduPlatform.Basket.API.Const;
using EduPlatform.Basket.API.Dtos;
using EduPlatform.Shared;
using EduPlatform.Shared.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace EduPlatform.Basket.API.Feature.Baskets.GetBasket;

public class GetBasketQueryHandler(
    BasketService basketService,
    IMapper mapper)
    : IRequestHandler<GetBasketQuery, ServiceResult<BasketDto>>
{
    public async Task<ServiceResult<BasketDto>> Handle(GetBasketQuery request,
        CancellationToken cancellationToken)
    {
        var basketAsString = await basketService.GetBasketFromCache(cancellationToken);

        if (string.IsNullOrEmpty(basketAsString))
            return ServiceResult<BasketDto>.Error("Basket not found", HttpStatusCode.NotFound);

        var basket = JsonSerializer.Deserialize<Data.Basket>(basketAsString)!;

        var basketDto = mapper.Map<BasketDto>(basket);


        return ServiceResult<BasketDto>.SuccessAsOk(basketDto);
    }
}