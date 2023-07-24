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
        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        var helloInput = new TranslationActivityInput("Hello", input.LanguageCode);

        TranslationActivityOutput helloResult;

        helloResult =
            await Workflow.ExecuteActivityAsync(() => Activities.TranslateTerm(helloInput), activityOptions);

        var helloMessage = $"{helloResult.Translation}, {input.Name}";

        var goodbyeInput = new TranslationActivityInput("Goodbye", input.LanguageCode);

        TranslationActivityOutput byeResult;

        byeResult =
            await Workflow.ExecuteActivityAsync(() => Activities.TranslateTerm(goodbyeInput), activityOptions);

        var goodbyeMessage = $"{byeResult.Translation}, {input.Name}";

        return new TranslationWorkflowOutput(helloMessage, goodbyeMessage);
    }
}
