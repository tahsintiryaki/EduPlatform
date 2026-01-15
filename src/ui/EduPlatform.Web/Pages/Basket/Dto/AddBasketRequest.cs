namespace EduPlatform.Web.Pages.Basket.Dto;

public record AddBasketRequest(
    Guid CourseId,
    string CourseName,
    decimal CoursePrice,
    string? ImageUrl
);