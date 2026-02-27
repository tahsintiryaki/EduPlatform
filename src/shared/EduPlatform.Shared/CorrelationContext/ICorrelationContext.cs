namespace EduPlatform.Shared.CorrelationContext;

public interface ICorrelationContext
{
    Guid CorrelationId { get; }
}