using System.Text.Json;
using EduPlatform.Bus.Event;
using EduPlatform.Payment.API.Feature.Payments.Create;
using EduPlatform.Payment.API.Repositories;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Payment.API.Consumers;

public class ReceivePaymentOnOrderCreatedEventConsumer(InboxDbContext inboxDbContext, IMediator mediator)
    : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        try
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

       
            var orderInbox = await inboxDbContext.OrderInboxes
                .Where(t => t.Processed == false && t.IdempotentToken == idempotentToken).FirstOrDefaultAsync();
            if (orderInbox != null)
            {
                var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.PayloadJson);

                #region paymentProcess

                CreatePaymentCommand createPaymentCommand = new CreatePaymentCommand(idempotentToken,
                    orderCreatedEvent!.OrderCode, orderCreatedEvent.PaymentToken,orderCreatedEvent.UserId, orderCreatedEvent.Amount);
                await mediator.Send(createPaymentCommand); 
                #endregion

                Console.WriteLine($"{orderCreatedEvent!.OrderCode} için ödeme işlemi başarıyla tamamlanmıştır.");
                orderInbox.Processed = true;
                orderInbox.ProcessDate = DateTime.UtcNow;
                await inboxDbContext.SaveChangesAsync();
                
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
      
    }
}