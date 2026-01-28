#region
using EduPlatform.Shared;
#endregion

namespace EduPlatform.Payment.API.Feature.Payments.Create;

public record CreatePaymentCommand(
    string OrderCode,
    string Type,
    string Token,
    string Last4,
    string Brand,
    int ExpMonth,
    int ExpYear,
    decimal Amount) : IRequestByServiceResult<CreatePaymentResponse>;
    
