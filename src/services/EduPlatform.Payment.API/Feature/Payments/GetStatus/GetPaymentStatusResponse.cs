namespace EduPlatform.Payment.API.Feature.Payments.GetStatus;

public record GetPaymentStatusResponse(Guid? PaymentId, bool IsPaid);