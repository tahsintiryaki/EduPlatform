#region

using EduPlatform.Shared;

#endregion

namespace EduPlatform.Payment.API.Feature.Payments.GetAllPaymentsByUserId;

public record GetAllPaymentsByUserIdQuery : IRequestByServiceResult<List<GetAllPaymentsByUserIdResponse>>;