using System.Net;
using System.Net.Http.Json;
using Temporalio.Activities;

namespace Application;

public static class Activities
{
    private static readonly HttpClient Client = new();

    // TODO Replace the return type (string) with the name of the record you defined as output
    // TODO Replace the input parameters with the record you defined as input
    [Activity]
    public static async Task<string> TranslateTerm(string inputTerm, string languageCode)
    {
        // TODO Change the parameters used in these two calls to UrlEncode with the appropriate fields from your record
        var lang = WebUtility.UrlEncode(languageCode);
        var term = WebUtility.UrlEncode(inputTerm);
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";

        // will throw if service is unreachable
        var response = await Client.GetAsync(url);

        // will throw if we successfully called the service, but it could not perform the translation (>400 status code)
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<string>() ?? "";

        // TODO Replace 'content' below with the record your using as output,
        return content;
    }
}
