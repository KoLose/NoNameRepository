using BlazorApp;
using BlazorApp.ApiRequest;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7244/") });
builder.Services.AddScoped<ApiRequest>();
builder.Services.AddScoped<AuthService>();
await builder.Build().RunAsync();
