using System.Text.Json;
using EduPlatform.Bus.Event;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace EduPlatform.Payment.Outbox.Worker.Service.Jobs;

public class PaymentOutboxPublishJob(PaymentOutboxReadDbContext _context, IPublishEndpoint publishEndpoint) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var typeName = typeof(PaymentSucceededEvent);
        var paymentOutboxes = await _context.PaymentOutboxes
            .Where(t => t.ProcessedDate == null && t.Type ==typeName.Name)
            .OrderBy(t => t.OccuredOn)
            .ToListAsync();
        foreach (var paymentOutbox in paymentOutboxes)
        {
            
            PaymentSucceededEvent paymentSucceededEvent = JsonSerializer.Deserialize<PaymentSucceededEvent>(paymentOutbox.Payload);
            if (paymentSucceededEvent != null)
            {
                await publishEndpoint.Publish(paymentSucceededEvent);
                Console.WriteLine($"{paymentSucceededEvent.OrderCode}Payment Succeeded Event pushed");
                paymentOutbox.ProcessedDate = DateTime.UtcNow;
                // _context.PaymentOutboxes.Update(paymentOutbox);
                await _context.SaveChangesAsync();
            }
        }

        await Console.Out.WriteLineAsync("Payment outbox table checked!");
    }
}