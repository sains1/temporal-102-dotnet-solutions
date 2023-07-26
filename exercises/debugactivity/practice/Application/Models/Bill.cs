namespace Application.Models;

public class Bill
{
    public required int CustomerId { get; set; }
    public required string OrderNumber { get; set; }
    public required string Description { get; set; }
    public required int Amount { get; set; }
}
