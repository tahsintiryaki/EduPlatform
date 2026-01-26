using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Domain.Entities;

namespace EduPlatform.Order.Persistence.Repositories;

public class OrderOutboxRepository(AppDbContext context) :GenericRepository<Guid,OrderOutbox>(context),IOrderOutboxRepository
{
    
}