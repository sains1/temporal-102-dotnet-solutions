using Application.Models;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace Application;

public static class Activities
{
    [Activity]
    public static Task<Distance> GetDistance(Address address)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("GetDistance invoked; determining distance to customer address");

        // this is a simulation, which calculates a fake (but consistent)
        // distance for a customer address based on its length. The value
        // will therefore be different when called with different addresses,
        // but will be the same across all invocations with the same address.
        var kilometers = address.Line1.Length + address.Line2.Length - 10;
        if (kilometers < 1)
        {
            kilometers = 5;
        }

        var distance = new Distance
        {
            Kilometers = kilometers
        };

        logger.LogDebug("GetDistance complete. Distance: {Distance}", distance.Kilometers);
        return Task.FromResult(distance);
    }

    [Activity]
    public static Task<OrderConfirmation> SendBill(Bill bill)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("SendBill invoked. Customer: {Customer}, Amount: {Amount}", bill.CustomerId, bill.Amount);

        var chargeAmount = bill.Amount;

        // This month's special offer: Get $5 off all orders over $30
        if (bill.Amount > 3000)
        {
            logger.LogInformation("Applying discount");

            chargeAmount =- 500; // reduce amount charged by 500 cents
        }

        // reject invalid amounts before calling the payment processor
        if (chargeAmount < 0)
        {
            throw new ArgumentException($"invalid charge amount: {chargeAmount} (must be above zero)");
        }

        // pretend we called a payment processing service here :-)

        var confirmation = new OrderConfirmation
        {
            OrderNumber = bill.OrderNumber,
            ConfirmationNumber = "AB9923",
            Status = "SUCCESS",
            BillingTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = chargeAmount
        };

        logger.LogDebug("SendBill complete. ConfirmationNumber: {Confirmation}", confirmation.ConfirmationNumber);

        return Task.FromResult(confirmation);
    }
}
