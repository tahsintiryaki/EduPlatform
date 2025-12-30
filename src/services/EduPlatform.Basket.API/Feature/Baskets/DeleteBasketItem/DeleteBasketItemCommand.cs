using EduPlatform.Shared;

namespace EduPlatform.Basket.API.Feature.Baskets.DeleteBasketItem;

public record DeleteBasketItemCommand(Guid Id) : IRequestByServiceResult;