#region

using EduPlatform.Bus.Event;
using MassTransit;

#endregion

namespace EduPlatform.Basket.API.Consumers;

public class DeleteBasketOnOrderCreatedEventConsumer(IServiceProvider serviceProvider) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        using var scope = serviceProvider.CreateScope();
        var basketService = scope.ServiceProvider.GetRequiredService<BasketService>();
        await basketService.DeleteBasket(context.Message.UserId);
    }
}