# Exercise 2: Observing Durable Execution

This readme was ported from the official golang tutorial [durable-execution/README.md](https://github.com/temporalio/edu-102-go-code/blob/main/exercises/durable-execution/README.md).

During this exercise, you will

- Create Workflow and Activity loggers
- Add logging statements to the code
- Add a Timer to the Workflow Definition
- Launch two Workers and run the Workflow
- Kill one of the Workers during Workflow Execution and observe that the remaining Worker completes the execution

We have the same project structure as in the previous exercise:

- `Application` - A .NET class library where we'll add our Workflows and Activities.
- `Client` - A .NET console application that interacts with the temporal server to start a Workflow.
- `Worker` - A .NET application with a background service that runs our temporal Worker.
- `Api` - A .NET API with a 'translation service' endpoint at `GET /translate`.

## Part A: Add Logging to the Workflow Code

1. Edit the [Application/Workflow.cs](./practice/Application/Workflow.cs) file
2. Define a Workflow logger at the top of the Workflow class
3. Add a new line after that to log a message at the Info level
   1. It should mention that the Workflow function has been invoked
   2. It should also include the name passed as input
4. Before each call to Execute Activity, log a message at Debug level
   1. This should should identify the word being translated
   2. It should also include the language code passed as input
5. Save your changes

## Part B: Add Logging to the Activity Code

1. Edit the [Application/Activity.cs](./practice/Application/Activities.cs) file
2. Define an Activity logger at the top of the Activity function
3. Insert a logging statement at the Info level just after this, so you'll know when the Activity is invoked.
   1. Include the term being translated and the language code as name-value pairs
4. Near the bottom of the function, use the Debug level to log the successful translation
   1. Include the translated term as a name-value pair
5. Save your changes

## Part C: Add a Timer to the Workflow

You will now add a Timer between the two Activity calls in the Workflow Definition, which will make it easier to observe durable execution in the next section.

1. After the statement where helloMessage is defined, but before the statement where goodbyeInput is defined, add a new statement that logs the message Sleeping between translation calls at the Debug level.
2. Just after the new log statement, use workflow.Sleep to set a Timer for 10 seconds

## Part D: Observe Durable Execution

It is typical to run Temporal applications using two or more Worker processes. Not only do additional Workers allow the application to scale, it also increases availability since another Worker can take over if a Worker crashes during Workflow Execution. You'll see this for yourself now and will learn more about how Temporal achieves this as you continue through the course.

Before proceeding, make sure that there are no Workers running for this or any previous exercise. Also, please read through all of these instructions before you begin, so that you'll know when and how to react.

1. In one terminal, start the translation microservice by running `dotnet run --project ./exercises/durableexecution/practice/Api/Api.csproj`

2. In another terminal, start the Worker by running `dotnet run --project ./exercises/durableexecution/practice/Worker/Worker.csproj`

3. In another terminal, start a second Worker by running `dotnet run --project ./exercises/durableexecution/practice/Worker/Worker.csproj`

4. In another terminal, execute the Workflow by running `dotnet run --project ./exercises/durableexecution/practice/Client/Client.csproj Tatiana sk` (replace Tatiana with your first name)

5. Observe the output in the terminal windows used by each worker.

6. As soon as you see a log message in one of the Worker terminals indicating that it has started the Timer, press Ctrl-C in that window to kill that Worker process.

7. Switch to the terminal window for the other Worker process. Within a few seconds, you should observe new output, indicating that it has resumed execution of the Workflow.

8. Once you see log output indicating that translation was successful, switch back to the terminal window where you started the Workflow.

After the final step, you should see the translated Hello and Goodbye messages, which confirms that Workflow Execution completed successfully despite the original Worker being killed.

Since you added logging code to the Workflow and Activity code, take a moment to look at what you see in the terminal windows for each Worker and think about what took place. You may also find it helpful to look at this Workflow Execution in the Web UI.

The microservice for this exercise logs each successful translation, and if you look at its terminal window, you will see that the service only translated Hello (the first Activity) once, even though the Worker was killed after this translation took place. In other words, Temporal did not re-execute the completed Activity when it restored the state of the Workflow Execution.
