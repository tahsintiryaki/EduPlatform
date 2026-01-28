#region

using EduPlatform.Web.Pages.Order.Dto;
using Refit;

#endregion

namespace EduPlatform.Web.Services.Refit;

public interface IPaymentRefitService
{
    [Post("/api/v1/payments")]
    Task<ApiResponse<object>> CreatePaymentAsync(PaymentDto paymentRequest);
   
}