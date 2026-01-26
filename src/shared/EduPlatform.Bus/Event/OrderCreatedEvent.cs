namespace EduPlatform.Bus.Event;

public record OrderCreatedEvent(Guid IdempotentToken, Guid OrderId, Guid UserId);