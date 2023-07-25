using System.Text.Encodings.Web;
using System.Text.Json;
using Application;
using Temporalio.Client;

if (args.Length < 2)
{
    throw new InvalidOperationException("Must specify name and language code as command-line arguments");
}

var client = await TemporalClient.ConnectAsync(new()
{
    TargetHost = "localhost:7233",
});

var options = new WorkflowOptions { TaskQueue = WorkflowConstants.TaskQueueName, Id = "translation-workflow" };
var input = new TranslationWorkflowInput(args[0], args[1]);

var result = await client.ExecuteWorkflowAsync((TranslationWorkflow x) => x.RunAsync(input), options);

var data = JsonSerializer.Serialize(result, new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
});

Console.WriteLine($"Workflow result: {data}");
