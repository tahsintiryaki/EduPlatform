#region

using AutoMapper;
using EduPlatform.Order.Application.UseCases.Orders.CreateOrder;
using EduPlatform.Order.Domain.Entities;

#endregion

namespace EduPlatform.Order.Application.UseCases.Orders;

public class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
    }
}