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

    [Fact]
    public async Task TestSuccessfulTranslateActivityGoodbyeLatvian()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Goodbye", "lv");

        var result = await env.RunAsync(() => Activities.TranslateTerm(input));

        Assert.Equal("Ardievu", result.Translation);
    }

    [Fact]
    public async Task TestFailedTranslateActivityBadLanguageCode()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Hello", "xq");

        Task<TranslationActivityOutput> Act() => env.RunAsync(() => Activities.TranslateTerm(input));

        var exception = await Assert.ThrowsAsync<HttpRequestException>(Act);
        Assert.Equal("Response status code does not indicate success: 400 (Bad Request).", exception.Message);
    }
}
