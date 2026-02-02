#region

using EduPlatform.Bus.Event;
using MassTransit;

#endregion

namespace EduPlatform.Basket.API.Consumers;

public class DeleteBasketOnOrderCreatedEventConsumer(IServiceProvider serviceProvider) : IConsumer<PaymentSucceededEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        using var scope = serviceProvider.CreateScope();
        var basketService = scope.ServiceProvider.GetRequiredService<BasketService>();
        await basketService.DeleteBasket(context.Message.UserId);
    }
}