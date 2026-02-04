namespace EduPlatform.Order.Application.UseCases.Orders.CreateOrder;

public record CreateOrderResponse(Guid idempotentToken, string OrderCode, string OrderStatus, decimal Amount, DateTime OrderDate);