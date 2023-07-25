using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
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
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("Translating term {Term} to {LanguageCode}", input.Term, input.LanguageCode);

        var lang = WebUtility.UrlEncode(input.LanguageCode);
        var term = WebUtility.UrlEncode(input.Term);
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";

        // will throw if service is unreachable
        var response = await Client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            // throw if we successfully called the service, but it could not perform the translation (>=400 status code)
            throw new HttpRequestException(
                $"HTTP Error {response.StatusCode}: {await response.Content.ReadFromJsonAsync<string>()}");
        }

        var content = await response.Content.ReadFromJsonAsync<string>() ?? "";
        logger.LogDebug("Translation successful. Translation: {Translation}", content);

        return new TranslationActivityOutput(content);
    }
}
