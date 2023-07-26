using Application;
using Temporalio.Common;
using Temporalio.Worker;

namespace Test;

public class LoanProcessingWorkflowTests
{

    [Fact]
    public async Task TestReplayWorkflowHistoryFromFile()
    {
        var replayer = new WorkflowReplayer(new WorkflowReplayerOptions().AddWorkflow<LoanProcessingWorkflow>());

        var json = await File.ReadAllTextAsync("history_for_original_execution.json");

        var result =
            await replayer.ReplayWorkflowAsync(WorkflowHistory.FromJson("loan-processing-workflow-customer-a100",
                json), false);

        Assert.Null(result.ReplayFailure);
    }
}
