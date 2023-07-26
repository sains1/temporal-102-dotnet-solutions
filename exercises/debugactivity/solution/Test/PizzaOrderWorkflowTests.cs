using Application;
using Application.Models;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.Exceptions;
using Temporalio.Testing;
using Temporalio.Worker;

namespace Test;

public class PizzaOrderWorkflowTests
{
    [Fact]
    public async Task TestSuccessfulPizzaOrder()
    {
        var taskQueueId = Guid.NewGuid().ToString();

        var order = CreatePizzaOrderForTest();

        // For this test, any address will have a distance of 10 kilometer, which
        // is within the delivery area
        var mockDistanceActivity = [Activity(nameof(Activities.GetDistance))](Address addr) =>
            new Distance { Kilometers = 10 };

        var mockBillActivity = [Activity(nameof(Activities.SendBill))](Bill bill) => new OrderConfirmation
        {
            OrderNumber = order.OrderNumber,
            ConfirmationNumber = "AB9923",
            Status = "SUCCESS",
            BillingTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = 2500
        };

        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        using var worker = new TemporalWorker(env.Client,
            new TemporalWorkerOptions(taskQueueId)
                .AddWorkflow<PizzaWorkflow>()
                .AddActivity(mockBillActivity)
                .AddActivity(mockDistanceActivity));

        var result = await worker.ExecuteAsync(async () =>
            await env.Client.ExecuteWorkflowAsync((PizzaWorkflow wf) => wf.RunAsync(order),
                new WorkflowOptions(id: $"wf-{Guid.NewGuid()}", taskQueue: taskQueueId)));

        Assert.Equal("Z1238", result.OrderNumber);
        Assert.Equal("SUCCESS", result.Status);
        Assert.Equal("AB9923", result.ConfirmationNumber);
        Assert.Equal(2500, result.Amount);
        Assert.True(result.BillingTimestamp > 0);
    }

    [Fact]
    public async Task TestFailedPizzaOrderCustomerOutsideDeliveryArea()
    {
        var taskQueueId = Guid.NewGuid().ToString();

        var order = CreatePizzaOrderForTest();

        var mockDistanceActivity = [Activity(nameof(Activities.GetDistance))](Address addr) =>
            new Distance { Kilometers = 30 };

        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        // NOTE there is no Mock for the SendBill Activity because it won't be
        // called, given that the Workflow returns an error due to the distance.
        using var worker = new TemporalWorker(env.Client,
            new TemporalWorkerOptions(taskQueueId)
                .AddWorkflow<PizzaWorkflow>()
                .AddActivity(mockDistanceActivity));

        Task<OrderConfirmation> Act() => worker.ExecuteAsync(async () =>
            await env.Client.ExecuteWorkflowAsync((PizzaWorkflow wf) => wf.RunAsync(order),
                new WorkflowOptions(id: $"wf-{Guid.NewGuid()}", taskQueue: taskQueueId)));

        // Since the Workflow failed, trying to access its result fails
        var exception = await Assert.ThrowsAsync<WorkflowFailedException>(Act);

        // When the Workflow returns an error during its execution, Temporal
        // wraps it in a Temporal-specific WorkflowExecutionError type, so we
        // must unwrap this to retrieve the error returned in the Workflow code.
        var inner = exception.InnerException as ApplicationFailureException;
        Assert.Equal("customer lives too far away for delivery", inner?.Failure?.Message);
    }

    private static PizzaOrder CreatePizzaOrderForTest()
    {
        var customer = new Customer
        {
            CustomerId = 12983, Name = "María García", Email = "maria1985@example.com", Phone = "415-555-7418"
        };

        var address = new Address
        {
            Line1 = "701 Mission Street",
            Line2 = "Apartment 9C",
            City = "San Francisco",
            State = "CA",
            PostalCode = "94103"
        };

        var p1 = new Pizza { Description = "Large, with pepperoni", Price = 1500 };
        var p2 = new Pizza { Description = "Small, with mushrooms and onions", Price = 1000 };

        var items = new List<Pizza> { p1, p2 };

        return new PizzaOrder
        {
            OrderNumber = "Z1238",
            Customer = customer,
            Items = items,
            Address = address,
            IsDelivery = true
        };
    }
}
