namespace Application.Models;

public class Address
{
    public required string Line1 { get; set; }
    public string Line2 { get; set; } = string.Empty;
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
}
