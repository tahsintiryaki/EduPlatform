using EduPlatform.Discount.API.Repositories;
using EduPlatform.Shared.Services;

namespace EduPlatform.Discount.API.Features.Discounts.GetDiscountByCode;

public class GetDiscountByCodeQueryHandler(AppDbContext context, IIdentityService identityService)
    : IRequestHandler<GetDiscountByCodeQuery, ServiceResult<GetDiscountByCodeQueryResponse>>
{
    public async Task<ServiceResult<GetDiscountByCodeQueryResponse>> Handle(GetDiscountByCodeQuery request,
        CancellationToken cancellationToken)
    {
        var discount = await context.Discounts.SingleOrDefaultAsync(x => x.Code == request.Code, cancellationToken);
        
        if (discount == null)
            return ServiceResult<GetDiscountByCodeQueryResponse>.Error("Discount not found", HttpStatusCode.NotFound);

        if (discount.Expired < DateTime.Now)
            return ServiceResult<GetDiscountByCodeQueryResponse>.Error("Discount is expired",
                HttpStatusCode.BadRequest);
        
        return ServiceResult<GetDiscountByCodeQueryResponse>.SuccessAsOk(
            new GetDiscountByCodeQueryResponse(discount.Code, discount.Rate));
    }
}