using System.Text.Json;
using EduPlatform.Bus.Event;
using EduPlatform.Order.Application.Contracts.Refit.PaymentService;
using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Application.UnitOfWork;
using EduPlatform.Order.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace EduPlatform.Order.Application.Consumers;

public class UpdateOrderStatusOnPaymentSucceededEventConsumer(
    IPublishEndpoint publishEndpoint,
    IPaymentInboxRepository paymentInboxRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : IConsumer<PaymentSucceededEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var idempotentToken = context.Message.IdempotentToken;
        var paymentInbox = paymentInboxRepository.Where(t => t.Id == idempotentToken).FirstOrDefault();

        if (paymentInbox is null)
        {
            //Inbox table insert
            paymentInbox = new PaymentInbox()
            {
                Id = idempotentToken,
                Processed = false,
                ProcessDate = null,
                PayloadJson = JsonSerializer.Serialize(context.Message)
            };
            paymentInboxRepository.Add(paymentInbox);
        }

        if (paymentInbox.Processed == true)
        {
            return;
        }

        //Order status will update
        await orderRepository.SetStatus(context.Message.OrderCode, context.Message.PaymentId,
            OrderStatus.Paid);
        paymentInbox.Processed = true;
        paymentInbox.ProcessDate = DateTime.UtcNow;
        Console.WriteLine($"{context.Message.OrderCode} order status updated");
        await unitOfWork.CommitAsync();
    }
}