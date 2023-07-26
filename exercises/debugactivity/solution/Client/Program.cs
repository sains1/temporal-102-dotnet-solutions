using System.Text.Encodings.Web;
using System.Text.Json;
using Application;
using Application.Models;
using Temporalio.Client;

var client = await TemporalClient.ConnectAsync(new()
{
    TargetHost = "localhost:7233",
});

var order = CreatePizzaOrder();

var result = await client.ExecuteWorkflowAsync((PizzaWorkflow w) => w.RunAsync(order),
    new WorkflowOptions
    {
        Id = $"pizza-workflow-order-{order.OrderNumber}", TaskQueue = WorkflowConstants.TaskQueueName
    });

var data = JsonSerializer.Serialize(result,
    new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

Console.WriteLine($"Workflow result: {data}");

PizzaOrder CreatePizzaOrder()
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

    var p1 = new Pizza { Description = "Large, with mushrooms and onions", Price = 1500 };
    var p2 = new Pizza { Description = "Small, with pepperoni", Price = 1200 };
    var p3 = new Pizza { Description = "Medium, with extra cheese", Price = 1300 };

    var pizzas = new List<Pizza> { p1, p2, p3 };

    return new PizzaOrder
    {
        OrderNumber = "Z1238",
        Customer = customer,
        Items = pizzas,
        Address = address,
        IsDelivery = true
    };
}
