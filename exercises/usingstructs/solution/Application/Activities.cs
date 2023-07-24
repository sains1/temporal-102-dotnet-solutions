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
        var lang = WebUtility.UrlEncode(input.LanguageCode);
        var term = WebUtility.UrlEncode(input.Term);
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";

        // will throw if service is unreachable
        var response = await Client.GetAsync(url);

        // will throw if we successfully called the service, but it could not perform the translation (>400 status code)
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return new TranslationActivityOutput(content);
    }
}
