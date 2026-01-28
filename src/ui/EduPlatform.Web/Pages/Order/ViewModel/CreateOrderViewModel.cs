#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using EduPlatform.Web.Pages.Basket.ViewModel;

#endregion

namespace EduPlatform.Web.Pages.Order.ViewModel;

public record CreateOrderViewModel
{
    public AddressViewModel Address { get; set; } = null!;

    // public CreatePaymentViewModel CreatePayment { get; set; } = null!;

    [ValidateNever] public List<OrderItemViewModel> OrderItems { get; set; } = [];


    [ValidateNever] public float? DiscountRate { get; set; }


    public decimal TotalPrice { get; set; }

    public static CreateOrderViewModel Empty => new()
    {
        Address = AddressViewModel.Empty,
        // CreatePayment = CreatePaymentViewModel.Empty
    };


    public void AddOrderItem(BasketItemViewModel basketItem)
    {
        OrderItems.Add(new OrderItemViewModel(basketItem.Id, basketItem.Name,
            basketItem.PriceByApplyDiscountRate ?? basketItem.Price));
    }
}