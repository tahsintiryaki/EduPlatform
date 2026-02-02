using System.Net;
using EduPlatform.Web.Extensions;
using EduPlatform.Web.Pages.Order.Dto;
using EduPlatform.Web.Pages.Order.ViewModel;
using EduPlatform.Web.Services.Refit;

namespace EduPlatform.Web.Services;

public class PaymentService(IPaymentRefitService paymentService, ILogger<OrderService> logger)
{
    public async Task<ServiceResult> CreatePayment(Guid idempotentToken, string orderCode, decimal amount, PaymentViewModel viewModel)
    {
        var paymentRequest = CreateFakePaymentModel(viewModel,orderCode, amount,idempotentToken);
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

    private static PaymentDto CreateFakePaymentModel(PaymentViewModel viewModel, string orderCode, decimal amount,Guid idempotentToken)
    {
        // ExpiryDate parsing (MM/YY veya MM/YYYY)
        var parts = viewModel.ExpiryDate.Split('/');
        var expMonth = int.Parse(parts[0]);
        var expYear = parts[1].Length == 2
            ? 2000 + int.Parse(parts[1])
            : int.Parse(parts[1]);
        var paymentToken = $"pm_fake_{Guid.NewGuid():N}".Substring(0, 18);
        return new PaymentDto(idempotentToken, "card", paymentToken, amount);
        
        // return new PaymentDto()
        // {
        //     // Type = "card",
        //     IdempotentToken =  idempotentToken,
        //     Token = $"pm_fake_{Guid.NewGuid():N}".Substring(0, 18),
        //     // Last4 = viewModel.CardNumber[^4..],
        //     // Brand = "VISA",
        //     // ExpMonth = expMonth,
        //     // ExpYear = expYear,
        //     Type =  orderCode,
        //     Amount =  amount,
        // };
    }
}