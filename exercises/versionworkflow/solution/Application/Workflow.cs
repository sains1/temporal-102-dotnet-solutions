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

        var patched = Workflow.Patched("MovedThankYouAfterLoop");
        if (!patched)
        {
            // For workflow executions started before the change, send thank you before the loop
            await Workflow.ExecuteActivityAsync(() => Activities.SendThankYouToCustomer(customerInfo), options);
        }

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

            await Workflow.DelayAsync(TimeSpan.FromSeconds(3));
        }

        if (patched)
        {
            // for workflow executions started after the change, send thank you after the loop
            await Workflow.ExecuteActivityAsync(() => Activities.SendThankYouToCustomer(customerInfo), options);
        }


        var result = $"Loan for customer '{customerInfo.CustomerId}' has been fully paid (total={totalPaid})";
        return result;
    }
}
