 
using EduPlatform.Order.Application.UseCases.Orders.CreateOrder;
 
namespace EduPlatform.Order.Application.UseCases.Orders.GetOrders;

public record GetOrdersResponse(DateTime Created, decimal TotalPrice, List<OrderItemDto> Items);