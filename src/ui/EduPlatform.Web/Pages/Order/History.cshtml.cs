#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduPlatform.Web.PageModels;
using EduPlatform.Web.Pages.Order.ViewModel;
using EduPlatform.Web.Pages.Order;
using EduPlatform.Web.Services;

#endregion

namespace EduPlatform.Web.Pages.Order;

[Authorize]
public class HistoryModel(OrderService orderService) : BasePageModel
{
    public List<OrderHistoryViewModel> OrderHistoryList { get; set; } = null!;

    public async Task<IActionResult> OnGet()
    {
        var response = await orderService.GetHistory();


        if (response.IsFail) return ErrorPage(response);

        OrderHistoryList = response.Data!;


        return Page();
    }
}