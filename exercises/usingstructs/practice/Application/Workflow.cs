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

        // TODO Create your Activity input record and populate it with the last two fields from ExecuteActivity call below

        // TODO Replace "string" below with your Activity output record type
        string helloResult;

        // TODO Use your input record in the ExecuteActivity call below
        helloResult = await Workflow.ExecuteActivityAsync(() => Activities.TranslateTerm("Hello", input.LanguageCode),
            activityOptions);

        // TODO Update the helloResult parameter below to use the Translation field from the Activity output record
        var helloMessage = $"{helloResult}, {input.Name}";

        // TODO Create your Activity input record and populate it with the last two fields from ExecuteActivity call below

        // TODO Replace "string" below with your Activity output record type
        string byeResult;

        // TODO Use your input record in the ExecuteActivity call below
        byeResult = await Workflow.ExecuteActivityAsync(() => Activities.TranslateTerm("Goodbye", input.LanguageCode),
            activityOptions);

        // TODO Update the byeResult parameter below to use the Translation field from the Activity output record
        var goodbyeMessage = $"{byeResult}, {input.Name}";

        return new TranslationWorkflowOutput(helloMessage, goodbyeMessage);
    }
}
