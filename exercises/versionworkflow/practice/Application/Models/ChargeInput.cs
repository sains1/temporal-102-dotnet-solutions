namespace Application.Models;

public class ChargeInput
{
    public required string CustomerId { get; set; }
    public required int Amount { get; set; }
    public required int PeriodNumber { get; set; }
    public required int NumberOfPeriods { get; set; }
}
