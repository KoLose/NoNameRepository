using System.Security.Claims;
using BlazorApp.ApiRequest;
using BlazorApp.ApiRequest.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Services;

public class AuthService : AuthenticationStateProvider
{
    private const string TokenKey = "authToken";
    private readonly IJSRuntime _jsRuntime;
    private readonly ApiRequest.ApiRequest _request;
    private ClaimsPrincipal _currentUser = CreateAnonymousUser();
    private bool _initialized;

    public AuthService(IJSRuntime jsRuntime, ApiRequest.ApiRequest request)
    {
        _jsRuntime = jsRuntime;
        _request = request;
    }

    public string Token { get; private set; } = string.Empty;
    public bool IsLoggedIn => _currentUser.Identity?.IsAuthenticated == true;
    public int UserId => int.TryParse(
        _currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId) ? userId : 0;
    public int RoleId => int.TryParse(
        _currentUser.FindFirst(ClaimTypes.Role)?.Value, out var roleId) ? roleId : 0;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await InitializeAsync();
        return new AuthenticationState(_currentUser);
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;
        Token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey)
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Token))
            return;

        var profile = await _request.Profile(Token);
        if (profile.status)
        {
            _currentUser = CreateUser(profile.user);
            return;
        }

        await ClearLocalSessionAsync();
    }

    public async Task<bool> LoginAsync(string token)
    {
        Token = token;
        _initialized = true;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

        var profile = await _request.Profile(token);
        if (!profile.status)
        {
            await ClearLocalSessionAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            return false;
        }

        _currentUser = CreateUser(profile.user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        return true;
    }

    public async Task<bool> RefreshProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
            return false;

        var profile = await _request.Profile(Token);
        if (!profile.status)
            return false;

        _currentUser = CreateUser(profile.user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        return true;
    }

    public async Task LogoutAsync()
    {
        if (!string.IsNullOrWhiteSpace(Token))
            await _request.Logout(Token);

        await ClearLocalSessionAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    private async Task ClearLocalSessionAsync()
    {
        Token = string.Empty;
        _currentUser = CreateAnonymousUser();
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }

    private static ClaimsPrincipal CreateUser(UserDataShort user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Name, user.name),
            new Claim("login", user.login),
            new Claim(ClaimTypes.Role, user.roleId.ToString()),
            new Claim("role_name", user.roleName)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "api-token"));
    }

    private static ClaimsPrincipal CreateAnonymousUser() =>
        new(new ClaimsIdentity());
}
