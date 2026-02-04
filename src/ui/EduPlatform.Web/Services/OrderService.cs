#region

using System.Net;
using System.Text.Json;
using EduPlatform.Web.Extensions;
using EduPlatform.Web.Pages.Order.Dto;
using EduPlatform.Web.Pages.Order.ViewModel;
using EduPlatform.Web.Services.Refit;

#endregion

namespace EduPlatform.Web.Services;

public class OrderService(IOrderRefitService orderService, ILogger<OrderService> logger)
{
    public async Task<ServiceResult<CreateOrderResponse>> CreateOrder(CreateOrderViewModel viewModel)
    {
     
        var idempotentToken = Guid.NewGuid();
        //createAddressDto
        var address = new AddressDto(viewModel.Address.Province, viewModel.Address.District,
            viewModel.Address.Street, viewModel.Address.ZipCode, viewModel.Address.Line);

        var payment = CreateFakePaymentModel(viewModel.Payment, viewModel.TotalPrice, idempotentToken);

        // orderItems
        var orderItems = viewModel.OrderItems.Select(x => new OrderItemDto(x.ProductId, x.ProductName, x.UnitPrice))
            .ToList();


        var createOrderRequest =
            new CreateOrderRequest(idempotentToken, viewModel.DiscountRate, address, payment, orderItems);

        var response = await orderService.CreateOrder(createOrderRequest);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
                return ServiceResult<CreateOrderResponse>.FailFromProblemDetails(response.Error);

            logger.LogProblemDetails(response.Error);
            return ServiceResult<CreateOrderResponse>.Error("An error occurred while creating the order");
        }

        var responseData = JsonSerializer.Deserialize<CreateOrderResponse>(
            response.Content.ToString(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return ServiceResult<CreateOrderResponse>.Success(responseData);
    }

    public async Task<ServiceResult<List<OrderHistoryViewModel>>> GetHistory()
    {
        var response = await orderService.GetOrders();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogProblemDetails(response.Error);
            return ServiceResult<List<OrderHistoryViewModel>>.Error(
                "An error occurred while getting the order history");
        }

        var orderHistoryList = new List<OrderHistoryViewModel>();


        foreach (var orderResponse in response.Content)
        {
            var newOrderHistory =
                new OrderHistoryViewModel(orderResponse.Created.ToLongDateString(),
                    orderResponse.TotalPrice.ToString("C"), orderResponse.OrderStatus);

            foreach (var orderItem in orderResponse.Items)
                newOrderHistory.AddItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice);

            orderHistoryList.Add(newOrderHistory);
        }


        return ServiceResult<List<OrderHistoryViewModel>>.Success(orderHistoryList);
    }

    private static PaymentDto CreateFakePaymentModel(PaymentViewModel viewModel, decimal amount, Guid idempotentToken)
    {
        // ExpiryDate parsing (MM/YY veya MM/YYYY)
        var parts = viewModel.ExpiryDate.Split('/');
        var expMonth = int.Parse(parts[0]);
        var expYear = parts[1].Length == 2
            ? 2000 + int.Parse(parts[1])
            : int.Parse(parts[1]);
        var paymentToken = $"pm_fake_{Guid.NewGuid():N}".Substring(0, 18);
        return new PaymentDto(idempotentToken, "card", paymentToken, amount);
    }
}