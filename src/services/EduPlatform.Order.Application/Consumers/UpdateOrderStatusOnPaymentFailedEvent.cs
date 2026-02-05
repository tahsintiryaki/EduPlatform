using EduPlatform.Bus.Event;
using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Domain.Entities;
using MassTransit;

namespace EduPlatform.Order.Application.Consumers;

public class UpdateOrderStatusOnPaymentFailedEvent(IOrderRepository orderRepository, IPaymentInboxRepository paymentInboxRepository):IConsumer<PaymentFailedEvent>
{
    public Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var order = orderRepository.Where(t => t.Code == context.Message.OrderCode).FirstOrDefault();
        if (order != null)
        {
            orderRepository.SetStatus(context.Message.OrderCode, context.Message.PaymentId, OrderStatus.Cancel);
        }

        return  Task.CompletedTask;
    }
}