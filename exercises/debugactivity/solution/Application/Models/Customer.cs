namespace Application.Models;

public class Customer
{
    public required int CustomerId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}
