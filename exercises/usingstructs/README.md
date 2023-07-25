# Exercise 1: Using Structs for Data

This readme was ported from the official golang tutorial [using-structs/README.md](https://github.com/temporalio/edu-102-go-code/blob/main/exercises/using-structs/README.md).

> Note - This exercise differs slightly from the README it was ported from. In the original, the intention is to teach the participant that using a `struct` as Workflow input is a more maintainable approach than adding or removing parameters. In C# I've chosen to use a `record` for this purpose, but you could just as easily use a `class`.

During this exercise, you will

- Define structs to represent input and output of an Activity Definition
- Update the Activity and Workflow code to use these structs
- Run the Workflow to ensure that it works as expected

We have the same project structure as in the previous exercise. Make your changes to the code in the `practice` subdirectory (look for `TODO` comments that will guide you to where you should make changes to the code). If you need a hint or want to verify your changes, look at the complete version in the `solution` subdirectory.

We have the following projects in the solution:

- `Application` - A .NET class library where we'll add our Workflows and Activities.
- `Client` - A .NET console application that interacts with the temporal server to start a Workflow.
- `Worker` - A .NET application with a background service that runs our temporal Worker.
- `Api` - A .NET API with a 'translation service' endpoint at `GET /translate`.

## Part A: Define the Activity Structs

This exercise provides an improved version of the translation Workflow used in Temporal 101. The Workflow has already been updated to follow the best practice of using structs to represent input parameters and return values. You'll apply what you've learned to do the same for the Activity.

Before continuing with the steps below, take a moment to look at the code in the [Applications/Workflow.cs](./practice/Application/Workflow.cs) file to see how we can use records as input and output to the Workflow. Finally, look at the [Client/Program.cs](./practice/Client/Program.cs) to see how the input parameters are created and passed into the Workflow.

> Note - As mentioned above I could just have easily used a `class` when porting this code from the go tutorial. Also note that I've chosen to co-locate the `records` in the same file as the `Workflow` but there's no reason you couldn't move them to their own file.

Once you're ready to implement something similar for the Activity, continue with the steps below:

1. Edit the [Application/Activities.cs](./practice/Application/Activities.cs) file
2. Define a record called `TranslationActivityInput` to use as an input parameter.
   1. Define a property named `Term` of type `string`
   2. Define a property named `LanguageCode` of type `string`
3. Define a record called `TranslationActivityOutput` to use for the result
   1. Define a property named `Translation` of type `string`
4. Save your changes

## Part B: Use the Structs in Your Activity

Now that you have defined the structs, we must update the Activity code to use them.

1. Edit the [Applications/Activities.cs](./practice/Application/Activities.cs) file
2. Replace the two input parameters in the `TranslateTerm` function with the record you defined as input
3. Replace the output type (string) in the `TranslateTerm` function with the name of the record you defined as output
4. At the end of the function create a `TranslationActivityOutput` record and populate its `Translation` field with the `content` variable, which holds the translation returned in the microservice call.
5. Return the record created in the previous step
6. Save your changes

## Part C: Update the Workflow Code

You've now updated the Activity code to use the structs. The next step is to update the Workflow code to use these structs where it passes input to the Activity and access its return value.

1. Edit the [Applications/Workflow.cs](./practice/Application/Workflow.cs) file
2. Add a new line to define a `TranslationActivityInput` record, populating it with the two fields (term and language code) currently passed as input to the first `ExecuteActivity` call
3. Change the variable type used to access the result the first call to `ExecuteActivity` from `string` to `TranslationActivityOutput`
4. Change that `ExecuteActivity` call to use the record as input instead of the two parameters it now uses
5. Update the `helloMessage` string so that it is based on the `Translation` field from the Activity output record
6. Repeat the previous four steps for the second call to `ExecuteActivity`, which translates "Goodbye"
7. Save your changes

## Part D: Run the Translation Workflow

Now that you've made the necessary changes, it's time to run the Workflow to ensure that it works as expected.

1. In one terminal, start the translation microservice by running:

```sh
dotnet run --project ./exercises/usingstructs/practice/Api/Api.csproj
```

2. In another terminal, start the Worker by running:

```sh
dotnet run --project ./exercises/usingstructs/practice/Worker/Worker.csproj
```

3. In another terminal, execute the Workflow by running:

> (replace `Pierre` with your first name), which should display customized greeting and farewell messages in French.

```sh
dotnet run --project ./exercises/usingstructs/practice/Client/Client.csproj Pierre fr
```

If your code didn't work as expected, go back and doublecheck your changes, possibly comparing them to the code in the `solution` directory.

It's common for a single Workflow Definition to be executed multiple times, each time using a different input. Feel free to experiment with this by specifying a different language code when starting the Workflow. The translation service currently supports the following languages:

- `de`: German
- `es`: Spanish
- `fr`: French
- `lv`: Latvian
- `mi`: Maori
- `sk`: Slovak
- `tr`: Turkish
- `zu`: Zulu
