#region

using EduPlatform.Web.Pages.Order.ViewModel;

#endregion

namespace EduPlatform.Web.Pages.Order.Dto;

public record GetOrderHistoryResponse(DateTime Created, decimal TotalPrice, string OrderStatus, List<OrderItemViewModel> Items);