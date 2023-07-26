namespace Application.Models;

public class PizzaOrder
{
    public required string OrderNumber { get; set; }
    public required Customer Customer { get; set; }
    public required List<Pizza> Items { get; set; }
    public required bool IsDelivery { get; set; }
    public required Address Address { get; set; }
}
