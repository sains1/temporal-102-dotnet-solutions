using Application.Models;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace Application;

[Workflow]
public class PizzaWorkflow
{
    [WorkflowRun]
    public async Task<OrderConfirmation> RunAsync(PizzaOrder order)
    {
        var options = new ActivityOptions
        {
            StartToCloseTimeout = TimeSpan.FromSeconds(5),
            RetryPolicy = new Temporalio.Common.RetryPolicy { MaximumInterval = TimeSpan.FromSeconds(10) },
        };

        var totalPrice = order.Items.Sum(pizza => pizza.Price);

        var distance = await Workflow.ExecuteActivityAsync(() => Activities.GetDistance(order.Address), options);

        if (order.IsDelivery && distance.Kilometers > 25)
        {
            throw new ApplicationFailureException("customer lives too far away for delivery");
        }

        // We use a short Timer duration here to avoid delaying the exercise
        await Workflow.DelayAsync(TimeSpan.FromSeconds(3));

        var bill = new Bill
        {
            CustomerId = order.Customer.CustomerId,
            OrderNumber = order.OrderNumber,
            Amount = totalPrice,
            Description = "Pizza",
        };

        var confirmation = await Workflow.ExecuteActivityAsync(() => Activities.SendBill(bill), options);

        return confirmation;
    }
}
