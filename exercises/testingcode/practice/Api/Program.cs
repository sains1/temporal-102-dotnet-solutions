using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var translations = new Dictionary<string, Dictionary<string, string>>
{
    ["de"] = new() { ["hello"] = "hallo", ["goodbye"] = "auf wiedersehen", ["thanks"] = "danke schön", },
    ["es"] = new() { ["hello"] = "hola", ["goodbye"] = "adiós", ["thanks"] = "gracias", },
    ["fr"] = new() { ["hello"] = "bonjour", ["goodbye"] = "au revoir", ["thanks"] = "merci", },
    ["lv"] = new() { ["hello"] = "sveiks", ["goodbye"] = "ardievu", ["thanks"] = "paldies", },
    ["mi"] = new() { ["hello"] = "kia ora", ["goodbye"] = "poroporoaki", ["thanks"] = "whakawhetai koe", },
    ["sk"] = new() { ["hello"] = "ahoj", ["goodbye"] = "zbohom", ["thanks"] = "ďakujem koe", },
    ["tr"] = new() { ["hello"] = "merhaba", ["goodbye"] = "güle güle", ["thanks"] = "teşekkür ederim", },
    ["zu"] = new() { ["hello"] = "hamba kahle", ["goodbye"] = "sawubona", ["thanks"] = "ngiyabonga", },
};

app.MapGet("/translate", ([FromQuery] string? lang, [FromQuery] string? term) =>
{
    if (string.IsNullOrEmpty(lang))
    {
        return Results.BadRequest(
            "Missing required 'lang' parameter.");
    }

    var language = lang.ToLower();
    if (!translations.ContainsKey(language))
    {
        return Results.BadRequest(
            $"Unknown language code '{language}'");
    }

    if (string.IsNullOrEmpty(term))
    {
        return Results.BadRequest("Missing required 'term' parameter.");
    }

    var key = term.ToLower();
    if (!translations[language].ContainsKey(key))
    {
        return Results.BadRequest(
            $"Unknown term '{term}' for language '{language}'");
    }

    var translation = translations[language][key];

    if (char.IsUpper(term[0]))
    {
        // if the phrase had an initial uppercase letter, reflect that in the translation
        translation = char.ToUpper(translation[0]) + translation[1..];
    }

    return Results.Ok(translation);
});

app.Run();
