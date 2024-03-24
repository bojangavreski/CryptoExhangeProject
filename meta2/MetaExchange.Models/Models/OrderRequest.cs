using System.ComponentModel.DataAnnotations;


public class OrderRequest
{
    [Required(ErrorMessage = "Amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "OrderType is required")]
    [RegularExpression("^(buy|sell)$", ErrorMessage = "OrderType must be 'buy' or 'sell'")]
    public string OrderType { get; set; }
}