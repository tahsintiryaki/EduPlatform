using EduPlatform.Discount.API.Repositories;
using EduPlatform.Shared.Services;

namespace EduPlatform.Discount.API.Features.Discounts.CreateDiscount;

public class CreateDiscountCommandHandler(AppDbContext context, IIdentityService identityService)
    : IRequestHandler<CreateDiscountCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        var existCodeForUser = await context.Discounts.AnyAsync(
            x => x.UserId.ToString() == request.UserId.ToString() && x.Code == request.Code, cancellationToken);
        
        if (existCodeForUser)
            return ServiceResult.Error("Discount code already exists for this user", HttpStatusCode.BadRequest);
        
        var discount = new Discount
        {
            Id = NewId.NextSequentialGuid(),
            Code = request.Code,
            Created = DateTime.Now,
            Rate = request.Rate,
            Expired = request.Expired,
            UserId = request.UserId
        };

        await context.Discounts.AddAsync(discount, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return ServiceResult.SuccessAsNoContent();
    }
}