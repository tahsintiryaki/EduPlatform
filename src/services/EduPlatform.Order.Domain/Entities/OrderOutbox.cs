namespace EduPlatform.Order.Domain.Entities;

public class OrderOutbox:BaseEntity<Guid>
{
    public DateTime OccuredOn { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
}