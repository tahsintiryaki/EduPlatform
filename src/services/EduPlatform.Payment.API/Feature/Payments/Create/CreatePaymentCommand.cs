#region

using EduPlatform.Shared;
using UdemyNewMicroservice.Payment.Api.Feature.Payments.Create;

#endregion

namespace EduPlatform.Payment.API.Feature.Payments.Create;

public record CreatePaymentCommand(
    string OrderCode,
    string CardNumber,
    string CardHolderName,
    string CardExpirationDate,
    string CardSecurityNumber,
    decimal Amount) : IRequestByServiceResult<CreatePaymentResponse>;