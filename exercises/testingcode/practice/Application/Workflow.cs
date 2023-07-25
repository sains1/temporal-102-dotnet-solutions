using Microsoft.Extensions.Logging;
using Temporalio.Workflows;

namespace Application;

public record TranslationWorkflowInput(string Name, string LanguageCode);
public record TranslationWorkflowOutput(string HelloMessage, string GoodbyeMessage);

[Workflow]
public class TranslationWorkflow
{
    [WorkflowRun]
    public async Task<TranslationWorkflowOutput> RunAsync(TranslationWorkflowInput input)
    {
        var logger = Workflow.Logger;

        logger.LogInformation("TranslationWorkflow invoked with name {Name}", input.Name);

        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        logger.LogDebug("Preparing to translate 'Hello', '{LanguageCode}'", input.LanguageCode);
        var helloInput = new TranslationActivityInput("Hello", input.LanguageCode);

        var helloResult =
            await Workflow.ExecuteActivityAsync(() => Activities.TranslateTerm(helloInput), activityOptions);

        var helloMessage = $"{helloResult.Translation}, {input.Name}";

        logger.LogDebug("Sleeping between translation calls");
        await Workflow.DelayAsync(TimeSpan.FromSeconds(10));

        logger.LogDebug("Preparing to translate 'Goodbye', '{LanguageCode}'", input.LanguageCode);
        var goodbyeInput = new TranslationActivityInput("Goodbye", input.LanguageCode);

        var byeResult =
            await Workflow.ExecuteActivityAsync(() => Activities.TranslateTerm(goodbyeInput), activityOptions);

        var goodbyeMessage = $"{byeResult.Translation}; {input.Name}";

        return new TranslationWorkflowOutput(helloMessage, goodbyeMessage);
    }
}
