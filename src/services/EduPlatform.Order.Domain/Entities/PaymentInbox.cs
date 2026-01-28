using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Order.Domain.Entities;

public class PaymentInbox:BaseEntity<Guid>
{
    public bool? Processed { get; set; }
    public DateTime? ProcessDate { get; set; }
    public string PayloadJson { get; set; }
}