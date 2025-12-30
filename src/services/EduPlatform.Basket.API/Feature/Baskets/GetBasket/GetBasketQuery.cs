using EduPlatform.Basket.API.Dtos;
using EduPlatform.Shared;

namespace EduPlatform.Basket.API.Feature.Baskets.GetBasket;

public record GetBasketQuery : IRequestByServiceResult<BasketDto>;