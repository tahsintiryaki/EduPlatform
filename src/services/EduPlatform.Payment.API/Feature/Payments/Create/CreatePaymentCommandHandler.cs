#region

using System.Net;
using System.Text.Json;
using EduPlatform.Bus.Event;
using EduPlatform.Payment.API.Repositories;
using EduPlatform.Shared;
using EduPlatform.Shared.Services;
using MassTransit;
using MediatR;

#endregion

namespace EduPlatform.Payment.API.Feature.Payments.Create;

public class CreatePaymentCommandHandler(
    InboxDbContext appDbContext,
    IIdentityService identityService,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<CreatePaymentCommand, ServiceResult<CreatePaymentResponse>>
{
    public async Task<ServiceResult<CreatePaymentResponse>> Handle(CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = identityService.UserId;
        // var userName = identityService.UserName;
        // var roles = identityService.Roles;
        
        var (isSuccess, errorMessage) = await ExternalPaymentProcessAsync(request);
        
        if (!isSuccess)
            return ServiceResult<CreatePaymentResponse>.Error("Payment Failed", errorMessage!,
                HttpStatusCode.BadRequest);
        var idempotentToken = NewId.NextGuid();

        var newPayment = new Repositories.Payment(userId, request.OrderCode, request.Amount);
        newPayment.SetStatus(PaymentStatus.Success);

        PaymentSucceededEvent paymentSucceededEvent = new PaymentSucceededEvent(
            idempotentToken,
             newPayment.OrderCode,
             newPayment.Id,
            userId);
        appDbContext.Payments.Add(newPayment);
        var paymentOutbox = new PaymentOutbox()
        {
            IdempotentToken = NewId.NextGuid(),
            OccuredOn = DateTime.UtcNow,
            Payload = JsonSerializer.Serialize(paymentSucceededEvent),
            ProcessedDate = null,
            Type = typeof(PaymentSucceededEvent).Name
        };
        await appDbContext.PaymentOutboxes.AddAsync(paymentOutbox);
        await appDbContext.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"{newPayment.OrderCode}Payment Succeeded Event added to Payment Outbox");
        return ServiceResult<CreatePaymentResponse>.SuccessAsOk(
            new CreatePaymentResponse(newPayment.Id, true, null));
    }


    private async Task<(bool isSuccess, string? errorMessage)> ExternalPaymentProcessAsync(CreatePaymentCommand createPaymentCommand)
    {
        // Simulate external payment processing logic
        await Task.Delay(1000); // Simulating a delay for the external service call
        return (true, null); // Assume the payment was successful

        //return (false,"Payment failed due to insufficient funds."); // Simulate a failure case
    }
}