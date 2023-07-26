namespace Application.Data;

public class CustomerInfo
{
    public required string CustomerId { get; set; }
    public required string Name { get; set; }
    public required string EmailAddress { get; set; }
    public required int Amount { get; set; }
    public required int NumberOfPeriods { get; set; }
}
