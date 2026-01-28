namespace EduPlatform.Bus.Event;

public record PaymentSucceededEvent(Guid IdempotentToken, string OrderCode,Guid PaymentId,Guid UserId);
