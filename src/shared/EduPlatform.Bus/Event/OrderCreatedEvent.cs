namespace EduPlatform.Bus.Event;

public record OrderCreatedEvent(Guid IdempotentToken, string OrderCode, string? PaymentToken,decimal Amount, Guid UserId);