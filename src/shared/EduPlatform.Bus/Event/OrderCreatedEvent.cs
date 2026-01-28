namespace EduPlatform.Bus.Event;

public record OrderCreatedEvent(Guid IdempotentToken, string OrderCode, Guid UserId);