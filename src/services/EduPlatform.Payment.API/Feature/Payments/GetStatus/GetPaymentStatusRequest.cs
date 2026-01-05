#region

using EduPlatform.Shared;

#endregion

namespace EduPlatform.Payment.API.Feature.Payments.GetStatus;

public record GetPaymentStatusRequest(string orderCode) : IRequestByServiceResult<GetPaymentStatusResponse>;