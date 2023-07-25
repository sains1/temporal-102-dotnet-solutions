# Exercise 3: Testing the Translation Workflow

This readme was ported from the official golang tutorial [testing-code/README.md](https://github.com/temporalio/edu-102-go-code/blob/main/exercises/testing-code/README.md).

During this exercise, you will

- Run a unit test provided for the `TranslateTerm` Activity
- Develop and run your own unit test for the `TranslateTerm` Activity
- Write assertions for a Workflow test
- Uncover, diagnose, and fix a bug in the Workflow Definition
- Observe the time-skipping feature in the Workflow test environment

Make your changes to the code in the `practice` subdirectory (look for `TODO` comments that will guide you to where you should make changes to the code). If you need a hint or want to verify your changes, look at the complete version in the `solution` subdirectory.

We have the same project structure as in the previous exercise with the notable addition of a `Test` project.

> Note: I've chosen to use XUnit as the testing framework, but you could presumably use NUnit, MSTest or any other testing framework you prefer.

## Part A: Running a Test

We have provided a unit test for the `TranslateTerm` Activity to get you started. This test verifies that the Activity correctly translates the term "Hello" to German. Take a moment to study the test, which you'll find in the [Test/TranslationActivityTests.cs](./practice/Test/TranslationActivityTests.cs) file. Since the test runs the Activity, which in turn calls the microservice to do the translation, you'll begin by starting that.

1. Open a new terminal and run `dotnet run --project ./exercises/testingcode/practice/Api/Api.csproj`
2. Run the `dotnet test ./exercises/testingcode/practice/Test/Test.csproj` command to execute the provided test

> Note - Don't worry if you see any warnings about skipped tests, you should see a message indicating that 1 test has passed and 1 test was skipped.

## Part B: Write and Run Another Test for the Activity

Now it's time to develop and run your own unit test, this time verifying that the Activity correctly supports the translation of a different word in a different language.

1. Edit the [Test/TranslationActivityTests.cs](./practice/Test/TranslationActivityTests.cs) file
2. Copy the `TestSuccessfulTranslateActivityHelloGerman` function, renaming the new function as `TestSuccessfulTranslateActivityGoodbyeLatvian`
3. Change the term for the input from `Hello` to `Goodbye`
4. Change the language code for the input from `de` (German) to `lv` (Latvian)
5. Assert that translation returned by the Activity is `Ardievu`

## Part C: Test the Activity with Invalid Input

In addition to verifying that your code behaves correctly when used as you intended, it is sometimes also helpful to verify its behavior with unexpected input. The example below does this, testing that the Activity returns the appropriate error when called with an invalid language code.

```csharp
[Fact]
public async Task TestFailedTranslateActivityBadLanguageCode()
{
    var env = new ActivityEnvironment();
    var input = new TranslationActivityInput("Hello", "xq");

    Task<TranslationActivityOutput> Act() => env.RunAsync(() => Activities.TranslateTerm(input));

    var exception = await Assert.ThrowsAsync<HttpRequestException>(Act);
    Assert.Equal("Response status code does not indicate success: 400 (Bad Request).", exception.Message);
}
```

Take a moment to study this code, and then continue with the following steps:

1. Copy the entire `TestFailedTranslateActivityBadLanguageCode` function provided above and paste it at the bottom of the [Test/TranslationActivityTests.cs](./practice/Test/TranslationActivityTests.cs) file
2. Save the changes
3. Run `dotnet test ./exercises/testingcode/practice/Test/Test.csproj` again to run this new test, in addition to the others

## Part D: Test a Workflow Definition

1. Edit the [Test/TranslationWorkflowTests.cs](./practice/Test/TranslationWorkflowTests.cs) file
2. Remove the Skip parameter from the Fact attribute to ensure the test runs.
3. Add assertions for the following conditions
   - The Workflow Execution has completed
   - The `HelloMessage` field in the result is `Bonjour, Pierre`
   - The `GoodbyeMessage` field in the result is `Au revoir, Pierre`
4. Save your changes
5. Run `dotnet test ./exercises/testingcode/practice/Test/Test.csproj` again to run this new test. This will fail, due to a bug in the Workflow Definition.
6. Find and fix the bug in the Workflow Definition
7. Run the `dotnet test ./exercises/testingcode/practice/Test/Test.csproj` command again to verify that you fixed the bug

There are two things to note about this test.

First, the test completes in under a second, even though the Workflow Definition contains a `DelayAsync` call that adds a 10-second delay to the Workflow Execution. This is because of the time-skipping feature provided by the test environment.

Second, calls to `AddActivity` near the top of the test indicate that the Activity Definitions are executed as part of this Workflow test. As you learned, you can test your Workflow Definition in isolation from the Activity implementations by using mocks. The optional exercise that follows provides an opportunity to try this for yourself.

## Part E (Optional) Using Mock Activities in a Workflow Test

If you have time and would like an additional challenge, continue with the following steps.

> Note: I'm not clear how mocks work in go so I've adapted the instructions heavily for .NET. In the .NET SDK activities are simply delegates, so instead of relying on any mocking library I've chosen to 'mock' activity invocations using local functions which seems to work well. Take at a look at the `solution` folder for an example of this.

1. Make a copy of the existing Workflow test file named `TranslationWorkflowMockTests.cs`
2. Edit the `TranslationWorkflowMockTests.cs` file
3. Rename the test function to `TestSuccessfulTranslationWithMocks`
4. Create a delegate that takes a `TranslationActivityInput` record as input and returns a `TranslationActivityOutput` record as output. The delegate will need to handle both the `Hello` and `Goodbye` input terms, returning the appropriate translation for each. Because Activities are named in temporal we need to name the delegate using the Activity attribute, making sure it matches the name of our 'real' activity.

e.g.

```cs
var mockTranslateTerm = [Activity(nameof(Activities.TranslateTerm))](TranslationActivityInput input) =>
{
    // if input == Hello => ...
    // if input == Goodbye => ...
    // else => ...
}
```

5. Replace the registration of the `TranslateTerm` Activity with our new mock
6. Save your changes
7. Run `dotnet test ./exercises/testingcode/practice/Test/Test.csproj` to run the tests
