namespace EduPlatform.Web.Pages.Order.Dto;

public record CreateOrderResponse(string OrderCode, string OrderStatus, decimal Amount, DateTime OrderDate);