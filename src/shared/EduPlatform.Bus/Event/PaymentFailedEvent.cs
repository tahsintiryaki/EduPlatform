namespace EduPlatform.Bus.Event;

public record PaymentFailedEvent(Guid IdempotentToken, string OrderCode,Guid PaymentId,Guid UserId, string Message);