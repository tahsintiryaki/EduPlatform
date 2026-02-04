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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace EduPlatform.Order.Application.UseCases.Orders.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrderOutboxRepository orderOutboxRepository,
    IIdentityService identityService,
    IPublishEndpoint publishEndpoint,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, ServiceResult<CreateOrderResponse>>
{
    // public async Task<ServiceResult<CreateOrderResponse>> Handle(CreateOrderCommand request,
    //     CancellationToken cancellationToken)
    // {
    //     var order = new Domain.Entities.Order();
    //     try
    //     {
    //         if (!request.Items.Any())
    //             return ServiceResult<CreateOrderResponse>.Error("Order items not found",
    //                 "Order must have at least one item",
    //                 HttpStatusCode.BadRequest);
    //
    //         order = orderRepository.Where(t => t.IdempotentToken == request.IdempotentToken).FirstOrDefault();
    //         if (order is null)
    //         {
    //             var newAddress = new Address
    //             {
    //                 Province = request.Address.Province,
    //                 District = request.Address.District,
    //                 Street = request.Address.Street,
    //                 ZipCode = request.Address.ZipCode,
    //                 Line = request.Address.Line
    //             };
    //
    //             order = Domain.Entities.Order.CreateUnPaidOrder(identityService.UserId, request.DiscountRate,
    //                 newAddress.Id, request.IdempotentToken);
    //             foreach (var orderItem in request.Items)
    //                 order.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice);
    //
    //             order.Address = newAddress;
    //             orderRepository.Add(order);
    //
    //             orderOutboxRepository.Add(new OrderOutbox()
    //             {
    //                 Id = NewId.NextGuid(),
    //                 OccuredOn = DateTime.UtcNow,
    //                 ProcessedDate = null,
    //                 Payload = JsonSerializer.Serialize(request),
    //                 Type = typeof(OrderCreatedEvent).Name,
    //             });
    //
    //             await unitOfWork.CommitAsync(cancellationToken);
    //         }
    //     }
    //     catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
    //                                        (sqlEx.Number == 2601 || sqlEx.Number == 2627))
    //     {
    //         Console.WriteLine($"duplicate kayıt:{ex.InnerException.Message}");
    //         // Aynı token başka request tarafından insert edilmiş
    //         // order = orderRepository.Where(t => t.IdempotentToken == request.IdempotentToken).FirstOrDefault();
    //     }
    //     
    //     await unitOfWork.CommitAsync(cancellationToken);
    //     order = orderRepository.Where(t => t.IdempotentToken == request.IdempotentToken).FirstOrDefault();
    //     return ServiceResult<CreateOrderResponse>.SuccessAsCreated(
    //         new CreateOrderResponse(order.IdempotentToken, order.Code, order.Status.ToString(), order.TotalPrice,
    //             order.Created), "");
    // }
    public async Task<ServiceResult<CreateOrderResponse>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.Items.Any())
            return ServiceResult<CreateOrderResponse>.Error(
                "Order items not found",
                "Order must have at least one item",
                HttpStatusCode.BadRequest);

        Domain.Entities.Order? order = null;

        try
        {
            // 1) Önce hızlı check (opsiyonel)
            order = orderRepository.Where(t => t.IdempotentToken == request.IdempotentToken).FirstOrDefault();
            if (order is not null)
                return Success(order);

            // 2) Oluştur
            var newAddress = new Address
            {
                Province = request.Address.Province,
                District = request.Address.District,
                Street = request.Address.Street,
                ZipCode = request.Address.ZipCode,
                Line = request.Address.Line
            };

            order = Domain.Entities.Order.CreateUnPaidOrder(
                identityService.UserId,
                request.DiscountRate,
                newAddress.Id,
                request.IdempotentToken);

            foreach (var item in request.Items)
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice);

            order.Address = newAddress;

            orderRepository.Add(order);
            OrderCreatedEvent orderCreatedEvent =
                new OrderCreatedEvent(request.IdempotentToken, order.Code, request.Payment.Token, order.TotalPrice,
                    identityService.UserId);
            orderOutboxRepository.Add(new OrderOutbox
            {
                Id = NewId.NextGuid(),
                OccuredOn = DateTime.UtcNow,
                ProcessedDate = null,
                Payload = JsonSerializer.Serialize(orderCreatedEvent),
                Type = orderCreatedEvent.GetType().Name,
                IdempotentToken = request.IdempotentToken // composite unique için şart
            });

            // 3) Commit: write işlemlerinde request cancellation ile commit’i kesme
            await unitOfWork.CommitAsync(cancellationToken);

            return Success(order);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                                           (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            // Unique violation -> demek ki başka attempt insert etti (veya etmeye çalıştı)
            order = orderRepository.Where(t => t.IdempotentToken == request.IdempotentToken).FirstOrDefault();

            if (order is null)
            {
                // Bu durumda genelde şu olur: diğer attempt de cancellation/rollback oldu.
                return ServiceResult<CreateOrderResponse>.Error(
                    "Order not created",
                    "Request was duplicated but no persisted order was found. Please retry.",
                    HttpStatusCode.Conflict);
            }

            return Success(order);
        }

        ServiceResult<CreateOrderResponse> Success(Domain.Entities.Order o) =>
            ServiceResult<CreateOrderResponse>.SuccessAsCreated(
                new CreateOrderResponse(o.IdempotentToken, o.Code, o.Status.ToString(), o.TotalPrice, o.Created),
                "");
    }
}