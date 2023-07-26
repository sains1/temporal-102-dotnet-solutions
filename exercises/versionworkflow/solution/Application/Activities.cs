using Application.Data;
using Application.Models;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace Application;

public static class Activities
{
    [Activity]
    public static Task<string> ChargeCustomer(ChargeInput input)
    {
        var logger = ActivityExecutionContext.Current.Logger;

        logger.LogInformation(
            "Charging customer. CustomerId: {Customer}, Amount: {Amount}, PeriodNumber: {Period}, NumberOfPeriods: {NumPeriods}",
            input.CustomerId, input.Amount, input.PeriodNumber, input.NumberOfPeriods);

        // just pretend that we charged them
        var confirmation = $"Charged ${input.Amount} to customer '{input.CustomerId}'";

        return Task.FromResult(confirmation);
    }

    [Activity]
    public static Task<string> SendThankYouToCustomer(CustomerInfo input)
    {
        var logger = ActivityExecutionContext.Current.Logger;

        logger.LogInformation(
            "Sending thank you message to customer. CustomerId: {Customer}, EmailAddress: {Email}",
            input.CustomerId, input.EmailAddress);

        // just pretend that we e-mailed them
        var confirmation = $"Sent thank you message to customer '{input.CustomerId}'";

        return Task.FromResult(confirmation);
    }
}
