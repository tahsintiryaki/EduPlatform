#region
using EduPlatform.Shared;
#endregion

namespace EduPlatform.Payment.API.Feature.Payments.Create;

public record CreatePaymentCommand(
    Guid IdempotentToken,
    Guid CorrelationId,
    string OrderCode,
    string? PaymentToken,
    Guid UserId,
    decimal Amount) : IRequestByServiceResult<CreatePaymentResponse>;
    
