using EduPlatform.Shared;

namespace EduPlatform.Basket.API.Feature.Baskets.AddBasketItem;

public record AddBasketItemCommand(Guid CourseId, string CourseName, decimal CoursePrice, string? ImageUrl)
    : IRequestByServiceResult;
 