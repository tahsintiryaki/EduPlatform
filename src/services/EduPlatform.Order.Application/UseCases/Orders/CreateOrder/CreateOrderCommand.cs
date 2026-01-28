using EduPlatform.Shared;


namespace EduPlatform.Order.Application.UseCases.Orders.CreateOrder;

public record CreateOrderCommand(float? DiscountRate, AddressDto Address, List<OrderItemDto> Items)
    : IRequestByServiceResult<CreateOrderResponse>;

public record AddressDto(string Province, string District, string Street, string ZipCode, string Line);

public record PaymentDto(string CardNumber, string CardHolderName, string Expiration, string Cvc, decimal Amount);