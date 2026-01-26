using EduPlatform.Order.Domain.Entities;

namespace EduPlatform.Order.Application.Contracts.Repositories;

public interface IOrderOutboxRepository:IGenericRepository<Guid,OrderOutbox>
{
    
}