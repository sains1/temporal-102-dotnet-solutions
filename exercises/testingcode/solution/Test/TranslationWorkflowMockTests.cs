using Application;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;

namespace Test;

public class TranslationWorkflowMockTests
{
    [Fact]
    public async Task TestSuccessfulTranslationWithMocks()
    {
        var taskQueueId = Guid.NewGuid().ToString();

        var mock = [Activity(nameof(Activities.TranslateTerm))] (TranslationActivityInput input) =>
        {
            var term = input.Term;
            var lang = input.LanguageCode;

            var result = (term, lang) switch
            {
                ("Hello", "fr") => new TranslationActivityOutput("Bonjour"),
                ("Goodbye", "fr") => new TranslationActivityOutput("Au revoir"),
                _ => throw new InvalidOperationException("Missing mock setup")
            };

            return Task.FromResult(result);
        };

        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        using var worker = new TemporalWorker(env.Client,
            new TemporalWorkerOptions(taskQueueId).AddWorkflow<TranslationWorkflow>()
                .AddActivity(mock));

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
