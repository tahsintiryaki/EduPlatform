#region

using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EduPlatform.Order.Persistence.Repositories;

public class OrderRepository(AppDbContext context)
    : GenericRepository<Guid, Domain.Entities.Order>(context), IOrderRepository
{
    public Task<List<Domain.Entities.Order>> GetOrderByBuyerId(Guid buyerId)
    {
        return Context.Orders.Include(x => x.OrderItems).Where(x => x.BuyerId == buyerId)
            .OrderByDescending(x => x.Created).ToListAsync();
    }

    public async Task SetStatus(string orderCode, Guid paymentId, OrderStatus status)
    {
        var order = await Context.Orders.FirstAsync(x => x.Code == orderCode);
        order.Status = status;
        order.PaymentId = paymentId;
        Context.Update(order);
    }
}