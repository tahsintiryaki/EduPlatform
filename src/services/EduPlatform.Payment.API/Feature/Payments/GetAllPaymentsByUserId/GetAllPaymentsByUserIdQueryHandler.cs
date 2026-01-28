#region

using EduPlatform.Payment.API.Repositories;
using EduPlatform.Shared;
using EduPlatform.Shared.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
 

#endregion

namespace EduPlatform.Payment.API.Feature.Payments.GetAllPaymentsByUserId;

public class GetAllPaymentsByUserIdQueryHandler(InboxDbContext context, IIdentityService identityService)
    : IRequestHandler<GetAllPaymentsByUserIdQuery, ServiceResult<List<GetAllPaymentsByUserIdResponse>>>
{
    public async Task<ServiceResult<List<GetAllPaymentsByUserIdResponse>>> Handle(
        GetAllPaymentsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId = identityService.UserId;

        var payments = await context.Payments
            .Where(x => x.UserId == userId)
            .Select(x => new GetAllPaymentsByUserIdResponse(
                x.Id,
                x.OrderCode,
                x.Amount.ToString("C"), // Format as currency
                x.Created,
                x.Status))
            .ToListAsync(cancellationToken);


        return ServiceResult<List<GetAllPaymentsByUserIdResponse>>.SuccessAsOk(payments);
    }
}