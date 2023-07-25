using Application;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;

namespace Test;

public class TranslationWorkflowTests
{
    [Fact]
    public async Task TestSuccessfulCompleteFrenchTranslation()
    {
        var taskQueueId = Guid.NewGuid().ToString();

        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        using var worker = new TemporalWorker(env.Client,
            new TemporalWorkerOptions(taskQueueId).AddWorkflow<TranslationWorkflow>()
                .AddActivity(Activities.TranslateTerm));

        await worker.ExecuteAsync(async () =>
        {
            var input = new TranslationWorkflowInput("Pierre", "fr");

            var result = await env.Client.ExecuteWorkflowAsync(
                (TranslationWorkflow wf) => wf.RunAsync(input),
                new(id: $"wf-{Guid.NewGuid()}", taskQueue: taskQueueId));

            Assert.Equal("Bonjour, Pierre", result.HelloMessage);
            Assert.Equal("Au revoir, Pierre", result.GoodbyeMessage);
        });
    }
}
