using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Payment.API.Repositories;

public class OrderInbox
{
    [Key]
    public Guid IdempotentToken { get; set; }
    public bool? Processed { get; set; }
    public DateTime? ProcessDate { get; set; }
    public string PayloadJson { get; set; }
}