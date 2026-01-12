#region

using EduPlatform.Order.Application.Contracts.Refit.PaymentService;
using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Application.UnitOfWork;
using EduPlatform.Order.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


#endregion

namespace EduPlatform.Order.Application.BackgroundServices;

public class CheckPaymentStatusOrderBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();

        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var orders = orderRepository.Where(x => x.Status == OrderStatus.WaitingForPayment)
                .ToList();

            //TO DO: This process will be done again with a bulk update. 
            foreach (var order in orders)
            {
                var paymentStatusResponse = await paymentService.GetStatusAsync(order.Code);

                if (paymentStatusResponse.IsPaid!)
                {
                    await orderRepository.SetStatus(order.Code, paymentStatusResponse.PaymentId!.Value,
                        OrderStatus.Paid);
                    await unitOfWork.CommitAsync(stoppingToken);
                }
            }

            await Task.Delay(15000, stoppingToken);
        }
    }
}