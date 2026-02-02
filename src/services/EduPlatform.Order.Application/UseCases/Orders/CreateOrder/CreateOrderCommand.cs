using EduPlatform.Shared;


namespace EduPlatform.Order.Application.UseCases.Orders.CreateOrder;

public record CreateOrderCommand(
    Guid IdempotentToken,
    float? DiscountRate,
    AddressDto Address,
    PaymentDto Payment,
    List<OrderItemDto> Items)
    : IRequestByServiceResult<CreateOrderResponse>;

public record AddressDto(string Province, string District, string Street, string ZipCode, string Line);

public record PaymentDto(Guid IdempotentToken,
    // string OrderCode,
    string Type,
    string Token,
    string Last4,
    string Brand,
    int ExpMonth,
    int ExpYear,
    decimal Amount);