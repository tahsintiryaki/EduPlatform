namespace EduPlatform.Order.Application.UseCases.Orders.CreateOrder;

public record CreateOrderResponse(string OrderCode, string OrderStatus, decimal Amount, DateTime OrderDate);