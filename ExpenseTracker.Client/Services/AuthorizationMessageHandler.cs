using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace ExpenseTracker.Client.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public AuthorizationMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // DelegatingHandler: stesso concetto dei middleware lato server (Program.cs),
    // ma applicato qui alle richieste HTTP IN USCITA dal Client.
    // Ogni chiamata fatta con l'HttpClient a cui questo handler è agganciato
    // passa PRIMA da qui: leggiamo il token salvato in localStorage e lo
    // alleghiamo come header Authorization, prima che la richiesta parta davvero.
    // base.SendAsync(...) = equivalente di _next(context) nei middleware server-side:
    // lascia proseguire la richiesta verso la sua destinazione reale.
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}