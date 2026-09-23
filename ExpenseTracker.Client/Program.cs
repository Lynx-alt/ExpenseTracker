using Blazored.LocalStorage;
using ExpenseTracker.Client;
using ExpenseTracker.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthorizationMessageHandler>();

builder.Services.AddHttpClient("ExpenseTrackerApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7071/");
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ExpenseTrackerApi"));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<CategoryService>();

await builder.Build().RunAsync();
