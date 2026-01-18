#region

using EduPlatform.Web.Pages.Order.Dto;
using Refit;

#endregion

namespace EduPlatform.Web.Services.Refit;

public interface IOrderRefitService
{
    //CreateOrder endpoint
    [Post("/api/v1/orders")]
    Task<ApiResponse<object>> CreateOrder(CreateOrderRequest request);

    [Get("/api/v1/orders")]
    Task<ApiResponse<List<GetOrderHistoryResponse>>> GetOrders();
}