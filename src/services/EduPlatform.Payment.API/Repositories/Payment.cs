#region

using MassTransit;

#endregion

namespace EduPlatform.Payment.API.Repositories;

public class Payment
{
    public Payment(Guid userId, string orderCode, decimal amount, Guid idempotentToken,string paymentToken)
    {
        Create(userId, orderCode, amount, idempotentToken,paymentToken);
    }

    public Guid Id { get; set; }
    public Guid IdempotentToken { get; set; }
    public string PaymentToken { get; set; }
    public Guid UserId { get; set; }
    public string OrderCode { get; set; }
    public DateTime Created { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }


    public void Create(Guid userId, string orderCode, decimal amount, Guid idempotentToken, string paymentToken)
    {
        Id = NewId.NextSequentialGuid();
        IdempotentToken = idempotentToken;
        Created = DateTime.UtcNow;
        UserId = userId;
        OrderCode = orderCode;
        Amount = amount;
        PaymentToken = paymentToken;
        Status = PaymentStatus.Pending;
    }

    public void SetStatus(PaymentStatus status)
    {
        Status = status;
    }
}

public enum PaymentStatus
{
    Success = 1,
    Failed = 2,
    Pending = 3
}