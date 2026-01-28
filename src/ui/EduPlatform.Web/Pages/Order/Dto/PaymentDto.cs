namespace EduPlatform.Web.Pages.Order.Dto;


public class PaymentDto
{
    public string OrderCode { get; set; }
    public decimal Amount  { get; set; }
    public string Type { get; set; } = "card";
    public string Token { get; set; } = null!;
    public string Last4 { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public int ExpMonth { get; set; }
    public int ExpYear { get; set; }
 
}