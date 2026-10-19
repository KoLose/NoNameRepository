using Microsoft.JSInterop;

namespace BlazorApp.Services;

public class AuthService
{
    private const string TokenKey = "authToken";
    private readonly IJSRuntime _jsRuntime;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string Token { get; private set; } = string.Empty;
    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

    public async Task InitializeAsync()
    {
        Token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey)
            ?? string.Empty;
    }

    public async Task LoginAsync(string token)
    {
        Token = token;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
    }

    public async Task LogoutAsync()
    {
        Token = string.Empty;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }
}
