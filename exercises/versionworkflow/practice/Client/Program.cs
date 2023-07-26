using Application;
using Application.Data;
using Temporalio.Client;


if (args.Length < 1)
{
    throw new ArgumentException("Must specify customer ID as command-line argument");
}

var customerId = args[0];
ICustomerInfoDatabase customerDb = new InMemoryCustomerInfoDatabase();

CustomerInfo customerInfo;
try
{
    customerInfo = await customerDb.Get(customerId);
}
catch (KeyNotFoundException ex)
{
    throw new AggregateException($"Error looking up customer ID {customerId}", ex);
}

var client = await TemporalClient.ConnectAsync(new()
{
    TargetHost = "localhost:7233",
});

var result = await client.ExecuteWorkflowAsync((LoanProcessingWorkflow w) => w.RunAsync(customerInfo),
    new WorkflowOptions
    {
        Id = $"loan-processing-workflow-customer-{customerInfo.CustomerId}",
        TaskQueue = WorkflowConstants.TaskQueueName
    });

Console.WriteLine("Workflow result: {0}", result);
