namespace EduPlatform.Payment.API.Feature.Payments.Create;

public record CreatePaymentResponse(Guid? PaymentId, bool Status, string? ErrorMessage);