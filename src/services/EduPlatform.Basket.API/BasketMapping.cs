using AutoMapper;
using EduPlatform.Basket.API.Data;
using EduPlatform.Basket.API.Dtos;

namespace EduPlatform.Basket.API;

public class BasketMapping : Profile
{
    public BasketMapping()
    {
        CreateMap<BasketDto, Data.Basket>().ReverseMap();
        CreateMap<BasketItemDto, BasketItem>().ReverseMap();
    }
}