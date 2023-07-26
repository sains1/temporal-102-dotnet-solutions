using Application.Data;
using Application.Models;
using Microsoft.Extensions.Logging;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace Application;

[Workflow]
public class LoanProcessingWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(CustomerInfo customerInfo)
    {
        var logger = Workflow.Logger;
        var options = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(60) };

        // TODO move this code when prompted
        await Workflow.ExecuteActivityAsync(() => Activities.SendThankYouToCustomer(customerInfo), options);

        var totalPaid = 0;
        for (var period = 1; period <= customerInfo.NumberOfPeriods; period++)
        {
            var chargeInput = new ChargeInput
            {
                CustomerId = customerInfo.CustomerId,
                Amount = customerInfo.Amount,
                PeriodNumber = period,
                NumberOfPeriods = customerInfo.NumberOfPeriods,
            };

            await Workflow.ExecuteActivityAsync(() => Activities.ChargeCustomer(chargeInput), options);

            totalPaid += chargeInput.Amount;

            logger.LogInformation("Payment complete. Period: {Period}, Total Paid: {TotalPaid}", period, totalPaid);

            // TODO change the duration of this Timer when prompted
            await Workflow.DelayAsync(TimeSpan.FromSeconds(3));
        }

        var result = $"Loan for customer '{customerInfo.CustomerId}' has been fully paid (total={totalPaid})";
        return result;
    }
}
