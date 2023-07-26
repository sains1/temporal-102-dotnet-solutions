using Application;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Temporalio.Client;
using Temporalio.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // NOTE - instead of using services.AddTemporalClient as in the other exercises, I've had to register the client
        //          manually in order to customise the LoggerFactory
        services.TryAddSingleton<ITemporalClient>(provider =>
            TemporalClient.CreateLazy(new TemporalClientConnectOptions
            {
                TargetHost = "localhost:7233",
                LoggerFactory = provider.GetRequiredService<ILoggerFactory>()
            }));

        services.AddHostedTemporalWorker(WorkflowConstants.TaskQueueName)
            .AddStaticActivities(typeof(Activities))
            .AddWorkflow<LoanProcessingWorkflow>();

    })
    .Build();

host.Run();
