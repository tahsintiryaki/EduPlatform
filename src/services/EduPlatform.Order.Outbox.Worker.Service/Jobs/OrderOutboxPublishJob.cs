using System.Text.Json;
using EduPlatform.Bus.Event;
using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Application.UnitOfWork;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace EduPlatform.Order.Outbox.Worker.Service.Jobs;

public class OrderOutboxPublishJob(IOrderOutboxRepository orderOutboxRepository,IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint):IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var orderOutboxes = await orderOutboxRepository
            .Where(t => t.ProcessedDate == null)
            .OrderBy(t => t.OccuredOn)
            .ToListAsync();
        foreach (var orderOutbox in orderOutboxes)
        {
            OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);
            if (orderCreatedEvent != null)
            {
                await publishEndpoint.Publish(orderCreatedEvent);
                orderOutbox.ProcessedDate = DateTime.UtcNow;
                orderOutboxRepository.Update(orderOutbox);
                await unitOfWork.CommitAsync();
            }
            await Console.Out.WriteLineAsync($" {orderCreatedEvent.OrderCode} orderCreatedEvent published.");
        }
       
    }
    
}