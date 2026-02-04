namespace EduPlatform.Web.Pages.Order.Dto;

public record CreateOrderRequest(
    Guid IdempotentToken,
    float? DiscountRate,
    AddressDto Address,
    PaymentDto Payment,
    List<OrderItemDto> Items);