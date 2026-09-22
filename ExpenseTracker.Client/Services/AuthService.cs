using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ExpenseTracker.Shared.DTOs;

namespace ExpenseTracker.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        await _localStorage.SetItemAsync("authToken", result!.Token);
        _authStateProvider.NotifyUserAuthentication(result.Token);

        return true;
    }

    public async Task<bool> LoginAsync(LoginDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        await _localStorage.SetItemAsync("authToken", result!.Token);
        _authStateProvider.NotifyUserAuthentication(result.Token);

        return true;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _authStateProvider.NotifyUserLogout();
    }

    private class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}