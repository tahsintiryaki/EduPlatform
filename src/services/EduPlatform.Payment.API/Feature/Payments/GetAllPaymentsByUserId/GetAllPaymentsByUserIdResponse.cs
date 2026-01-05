#region

using EduPlatform.Payment.API.Repositories;

#endregion

namespace EduPlatform.Payment.API.Feature.Payments.GetAllPaymentsByUserId;

public record GetAllPaymentsByUserIdResponse(
    Guid Id,
    string OrderCode,
    string Amount,
    DateTime Created,
    PaymentStatus Status);