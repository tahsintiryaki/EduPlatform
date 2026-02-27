namespace EduPlatform.Bus;

public interface ICorrelationId
{
    public Guid CorrelationId { get; init; }
}