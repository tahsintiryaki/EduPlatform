using System.Text.Json;
using EduPlatform.Bus.Event;
using EduPlatform.Payment.API.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Payment.API.Consumers;

public class ReceivePaymentOnOrderCreatedEventConsumer(InboxDbContext inboxDbContext) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var idempotentToken = context.Message.IdempotentToken;
        var result = await inboxDbContext.OrderInboxes.AnyAsync(i => i.IdempotentToken == idempotentToken);
        if (!result)
        {
            await inboxDbContext.OrderInboxes.AddAsync(new OrderInbox()
            {
                IdempotentToken = context.Message.IdempotentToken,
                Processed = false,
                ProcessDate = null,
                PayloadJson = JsonSerializer.Serialize<OrderCreatedEvent>(context.Message),
            });
            await inboxDbContext.SaveChangesAsync();
        }

        //Idempotency uygulanacak.
        var orderInbox = await inboxDbContext.OrderInboxes
            .Where(t => t.Processed == false && t.IdempotentToken == idempotentToken).FirstOrDefaultAsync();
        if (orderInbox != null)
        {
            var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.PayloadJson);
            Console.WriteLine($"{orderCreatedEvent?.OrderId} için ödeme işlemi başarıyla tamamlanmıştır.");
            orderInbox.Processed = true;
            orderInbox.ProcessDate = DateTime.UtcNow;
            await inboxDbContext.SaveChangesAsync();
        }

   
    }
}