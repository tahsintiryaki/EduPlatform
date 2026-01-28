 
using EduPlatform.Order.Application.UseCases.Orders.CreateOrder;
using EduPlatform.Order.Domain.Entities;

namespace EduPlatform.Order.Application.UseCases.Orders.GetOrders;

public record GetOrdersResponse(DateTime Created, decimal TotalPrice, string OrderStatus, List<OrderItemDto> Items);