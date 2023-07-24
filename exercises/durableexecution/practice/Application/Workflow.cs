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
        // TODO define the Workflow logger here

        // TODO Log, at the Info level, when the Workflow function is invoked
        //      and be sure to include the name passed as input

        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        // TODO Log, at the Debug level, a message about the Activity to be executed,
        //      be sure to include the language code passed as input
        var helloInput = new TranslationActivityInput("Hello", input.LanguageCode);

        var helloResult =
            await Workflow.ExecuteActivityAsync(() => Activities.TranslateTerm(helloInput), activityOptions);

        var helloMessage = $"{helloResult.Translation}, {input.Name}";

        // TODO: (Part C): log a message at the Debug level and then start a Timer for 10 seconds

        var goodbyeInput = new TranslationActivityInput("Goodbye", input.LanguageCode);

        var goodbyeResult =
            await Workflow.ExecuteActivityAsync(() => Activities.TranslateTerm(goodbyeInput), activityOptions);

        var goodbyeMessage = $"{goodbyeResult.Translation}, {input.Name}";

        return new TranslationWorkflowOutput(helloMessage, goodbyeMessage);
    }
}
