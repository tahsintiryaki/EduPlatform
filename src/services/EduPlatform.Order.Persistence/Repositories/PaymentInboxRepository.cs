using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Domain.Entities;

namespace EduPlatform.Order.Persistence.Repositories;

public class PaymentInboxRepository(AppDbContext context) :GenericRepository<Guid, PaymentInbox>(context),IPaymentInboxRepository
{
    
}