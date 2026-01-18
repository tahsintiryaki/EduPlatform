#region
using Refit;
#endregion

namespace EduPlatform.Order.Application.Contracts.Refit.PaymentService;

public interface IPaymentService
{
    [Post("/api/v1/payments")]
    Task<CreatePaymentResponse> CreateAsync(CreatePaymentRequest paymentRequest);

    [Get("/api/v1/payments/status/{orderCode}")]
    Task<GetPaymentStatusResponse> GetStatusAsync(string orderCode);
}