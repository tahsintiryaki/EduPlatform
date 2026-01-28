using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Payment.Outbox.Worker.Service;

public class PaymentOutbox
{
    [Key]
    public Guid IdempotentToken { get; set; }
    public DateTime OccuredOn { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
}