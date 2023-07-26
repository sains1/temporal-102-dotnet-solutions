using Application;
using Temporalio.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTemporalClient(x =>
        {
            x.TargetHost = "localhost:7233";
        });

        services.AddHostedTemporalWorker(WorkflowConstants.TaskQueueName)
            .AddStaticActivities(typeof(Activities))
            .AddWorkflow<PizzaWorkflow>();

    })
    .Build();

host.Run();
