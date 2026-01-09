#region

using EduPlatform.Bus.Event;
using EduPlatform.Discount.API.Features.Discounts;
using EduPlatform.Discount.API.Repositories;

#endregion

namespace EduPlatform.Discount.API.Consumers;

public class CreateDiscountOnOrderCreatedEventConsumer(IServiceProvider serviceProvider) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var discount = new Features.Discounts.Discount
            {
                Id = NewId.NextSequentialGuid(),
                Code = DiscountCodeGenerator.Generate(),
                Created = DateTime.Now,
                Rate = 0.1f,
                Expired = DateTime.Now.AddMonths(1),
                UserId = context.Message.UserId
            };

            await appDbContext.Discounts.AddAsync(discount);

            await appDbContext.SaveChangesAsync();
        }
    }
}