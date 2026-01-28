#region

using EduPlatform.Web.PageModels;
using EduPlatform.Web.Pages.Order.ViewModel;
using EduPlatform.Web.Services;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace EduPlatform.Web.Pages.Order;

public class CreateModel(BasketService basketService, OrderService orderService, PaymentService paymentService)
    : BasePageModel
{
    [BindProperty] public CreateOrderViewModel CreateOrderViewModel { get; set; } = CreateOrderViewModel.Empty;
    [BindProperty] public CreatePaymentViewModel CreatePaymentViewModel { get; set; } = CreatePaymentViewModel.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadInitialFormData();

        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        await LoadInitialFormData();
        if (!ModelState.IsValid) return Page();
        
        var orderResult = await orderService.CreateOrder(CreateOrderViewModel);

        if (orderResult.IsFail)
        {
            return ErrorPage(orderResult);
        }

        //Request payment service
        var paymentResult = await paymentService.CreatePayment(orderResult!.Data!.OrderCode,orderResult!.Data!.Amount,CreatePaymentViewModel);
        if (paymentResult.IsFail)
        {
            return ErrorPage(paymentResult);
        }

        return SuccessPage("order created successfully", "/Order/Result");
    }

    private async Task LoadInitialFormData()
    {
        var basketAsResult = await basketService.GetBasketsAsync();

        if (basketAsResult.IsFail)
        {
            ErrorPage(basketAsResult);
            return;
        }

        CreateOrderViewModel.TotalPrice =
            basketAsResult.Data.TotalPriceWithAppliedDiscount ?? basketAsResult.Data!.TotalPrice;

        CreateOrderViewModel.DiscountRate = basketAsResult.Data.DiscountRate;
        foreach (var basketItem in basketAsResult.Data!.Items) CreateOrderViewModel.AddOrderItem(basketItem);
    }
}