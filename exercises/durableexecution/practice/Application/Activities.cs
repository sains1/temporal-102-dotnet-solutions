using System.Net;
using Temporalio.Activities;

namespace Application;

public record TranslationActivityInput(string Term, string LanguageCode);
public record TranslationActivityOutput(string Translation);

public static class Activities
{
    private static readonly HttpClient Client = new();

    [Activity]
    public static async Task<TranslationActivityOutput> TranslateTerm(TranslationActivityInput input)
    {
        // TODO Define an Activity logger

        // TODO log Activity invocation, at the Info level, and include the term being
        //      translated and the language code as name-value pairs

        var lang = WebUtility.UrlEncode(input.LanguageCode);
        var term = WebUtility.UrlEncode(input.Term);
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";

        // will throw if service is unreachable
        var response = await Client.GetAsync(url);

        // will throw if we successfully called the service, but it could not perform the translation (>400 status code)
        response.EnsureSuccessStatusCode();

        // TODO  use the Debug level to log the successful translation and include the
        //       translated term as a name-value pair
        var content = await response.Content.ReadAsStringAsync();

        return new TranslationActivityOutput(content);
    }
}
