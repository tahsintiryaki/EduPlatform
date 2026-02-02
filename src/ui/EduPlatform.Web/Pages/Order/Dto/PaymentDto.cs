namespace EduPlatform.Web.Pages.Order.Dto;


public record PaymentDto(Guid IdempotentToken,
    // string OrderCode,
    string Type,
    string Token,
    // string Last4,
    // string Brand,
    // int ExpMonth,
    // int ExpYear,
    decimal Amount);