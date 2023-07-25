using Application;
using Temporalio.Testing;

namespace Test;

public class TranslationActivityTests
{
    [Fact]
    public async Task TestSuccessfulTranslateActivityHelloGerman()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Hello", "de");

        var result = await env.RunAsync(() => Activities.TranslateTerm(input));

        Assert.Equal("Hallo", result.Translation);
    }
}
