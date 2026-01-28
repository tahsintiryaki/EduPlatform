#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace EduPlatform.Web.Pages.Order.ViewModel;

public record CreatePaymentViewModel
{
    [Display(Name = "Card Number")] public string CardNumber { get; set; } = null!;

    [Display(Name = "Cardholder Name")] public string CardHolderName { get; set; } = null!;

    [Display(Name = "Expiry Date")] public string ExpiryDate { get; set; } = null!;

    [Display(Name = "CVV")] public string Cvv { get; set; } = null!;

    [Display(Name = "Payment Amount")] public decimal Amount { get; set; }

    public static CreatePaymentViewModel Empty => new();
}