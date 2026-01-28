using System.Net;
using EduPlatform.Web.Extensions;
using EduPlatform.Web.Pages.Order.Dto;
using EduPlatform.Web.Pages.Order.ViewModel;
using EduPlatform.Web.Services.Refit;

namespace EduPlatform.Web.Services;

public class PaymentService(IPaymentRefitService paymentService, ILogger<OrderService> logger)
{
    public async Task<ServiceResult> CreatePayment(string orderCode, decimal amount, CreatePaymentViewModel viewModel)
    {
        var paymentRequest = CreateFakePaymentModel(viewModel,orderCode, amount);
        var response = await paymentService.CreatePaymentAsync(paymentRequest);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
                return ServiceResult.FailFromProblemDetails(response.Error);

            logger.LogProblemDetails(response.Error);
            return ServiceResult.Error("An error occurred while creating the order");
        }

        return ServiceResult.Success();
    }

    private static PaymentDto CreateFakePaymentModel(CreatePaymentViewModel viewModel, string orderCode, decimal amount)
    {
        // ExpiryDate parsing (MM/YY veya MM/YYYY)
        var parts = viewModel.ExpiryDate.Split('/');
        var expMonth = int.Parse(parts[0]);
        var expYear = parts[1].Length == 2
            ? 2000 + int.Parse(parts[1])
            : int.Parse(parts[1]);

        return new PaymentDto()
        {
            Type = "card",
            Token = $"pm_fake_{Guid.NewGuid():N}".Substring(0, 18),
            Last4 = viewModel.CardNumber[^4..],
            Brand = "VISA",
            ExpMonth = expMonth,
            ExpYear = expYear,
            OrderCode =  orderCode,
            Amount =  amount,
        };
    }
}