#region

using Asp.Versioning.Builder;
using EduPlatform.Payment.API.Feature.Payments.Create;
using EduPlatform.Payment.API.Feature.Payments.GetAllPaymentsByUserId;
using EduPlatform.Payment.API.Feature.Payments.GetStatus;
using UdemyNewMicroservice.Payment.Api.Feature.Payments.Create;

#endregion

namespace UdemyNewMicroservice.Payment.Api.Feature.Payments;

public static class PaymentEndpointExt
{
    public static void AddPaymentGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        app.MapGroup("api/v{version:apiVersion}/payments").WithTags("payments").WithApiVersionSet(apiVersionSet)
            .CreatePaymentGroupItemEndpoint().GetAllPaymentsByUserIdGroupItemEndpoint()
            .GetPaymentStatusGroupItemEndpoint();
    }
}