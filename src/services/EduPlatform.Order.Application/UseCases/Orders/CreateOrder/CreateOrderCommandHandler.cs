using System.Net;
using System.Text.Json;
using EduPlatform.Bus.Event;
using EduPlatform.Order.Application.Contracts.Refit.PaymentService;
using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Application.UnitOfWork;
using EduPlatform.Order.Domain.Entities;
using EduPlatform.Shared;
using EduPlatform.Shared.Services;
using MassTransit;
using MediatR;


namespace EduPlatform.Order.Application.UseCases.Orders.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrderOutboxRepository orderOutboxRepository,
    IIdentityService identityService,
    IPublishEndpoint publishEndpoint,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!request.Items.Any())
            return ServiceResult.Error("Order items not found", "Order must have at least one item",
                HttpStatusCode.BadRequest);


        var newAddress = new Address
        {
            Province = request.Address.Province,
            District = request.Address.District,
            Street = request.Address.Street,
            ZipCode = request.Address.ZipCode,
            Line = request.Address.Line
        };


        var order = Domain.Entities.Order.CreateUnPaidOrder(identityService.UserId, request.DiscountRate,
            newAddress.Id);
        foreach (var orderItem in request.Items)
            order.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice);


        order.Address = newAddress;


        orderRepository.Add(order);
        await unitOfWork.CommitAsync(cancellationToken);

        var paymentRequest = new CreatePaymentRequest(order.Code, request.Payment.CardNumber,
            request.Payment.CardHolderName, request.Payment.Expiration, request.Payment.Cvc, order.TotalPrice);
        var paymentResponse = await paymentService.CreateAsync(paymentRequest);


        if (!paymentResponse.Status)
            return ServiceResult.Error(paymentResponse.ErrorMessage!, HttpStatusCode.InternalServerError);

        order.SetPaidStatus(paymentResponse.PaymentId!.Value);

        orderRepository.Update(order);
        await unitOfWork.CommitAsync(cancellationToken);
        Guid idempotentToken = NewId.NextSequentialGuid();
        OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent(idempotentToken, order.Id, identityService.UserId);
        orderOutboxRepository.Add(new OrderOutbox()
        {
            Id = idempotentToken,
            OccuredOn = DateTime.UtcNow,
            Type =  orderCreatedEvent.GetType().Name,
            ProcessedDate = null,
            Payload = JsonSerializer.Serialize(orderCreatedEvent),
        });
        await unitOfWork.CommitAsync(cancellationToken);
        // await publishEndpoint.Publish(new OrderCreatedEvent(order.Id, identityService.UserId),
        //     cancellationToken);

        return ServiceResult.SuccessAsNoContent();
    }
}