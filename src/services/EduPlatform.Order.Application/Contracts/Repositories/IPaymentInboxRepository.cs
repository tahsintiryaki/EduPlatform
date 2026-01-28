using EduPlatform.Order.Domain.Entities;

namespace EduPlatform.Order.Application.Contracts.Repositories;

public interface IPaymentInboxRepository:IGenericRepository<Guid,PaymentInbox>
{
    
}