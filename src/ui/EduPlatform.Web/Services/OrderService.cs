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
        //createAddressDto
        var address = new AddressDto(viewModel.Address.Province, viewModel.Address.District,
            viewModel.Address.Street, viewModel.Address.ZipCode, viewModel.Address.Line);


        //paymentDto
        // var payment = new PaymentDto(viewModel.Payment.CardNumber, viewModel.Payment.CardHolderName,
        //     viewModel.Payment.ExpiryDate, viewModel.Payment.Cvv, viewModel.TotalPrice);


        // orderItems
        var orderItems = viewModel.OrderItems.Select(x => new OrderItemDto(x.ProductId, x.ProductName, x.UnitPrice))
            .ToList();


        var createOrderRequest = new CreateOrderRequest(viewModel.DiscountRate, address, orderItems);


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
                    orderResponse.TotalPrice.ToString("C"),orderResponse.OrderStatus);

            foreach (var orderItem in orderResponse.Items)
                newOrderHistory.AddItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice);

            orderHistoryList.Add(newOrderHistory);
        }


        return ServiceResult<List<OrderHistoryViewModel>>.Success(orderHistoryList);
    }
}