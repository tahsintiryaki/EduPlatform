using EduPlatform.Shared;

namespace EduPlatform.Order.Application.UseCases.Orders.GetOrders;

public record GetOrdersQuery : IRequestByServiceResult<List<GetOrdersResponse>>;