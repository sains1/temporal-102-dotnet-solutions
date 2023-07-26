namespace Application.Models;

public class OrderConfirmation
{
    public required string OrderNumber { get; set; }
    public required string Status { get; set; }
    public required string ConfirmationNumber { get; set; }
    public required long BillingTimestamp { get; set; }
    public required int Amount { get; set; }
}
