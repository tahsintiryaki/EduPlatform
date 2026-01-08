namespace EduPlatform.Bus.Event;

public record OrderCreatedEvent(Guid OrderId, Guid UserId);