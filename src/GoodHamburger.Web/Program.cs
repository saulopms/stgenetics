using GoodHamburger.Web.Components;
using GoodHamburger.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<AuthHttpMessageHandler>();

var apiBase = new Uri(builder.Configuration["ApiSettings:BaseUrl"]
                      ?? "http://localhost:5022/");

builder.Services
    .AddHttpClient<AuthApiClient>(c => c.BaseAddress = apiBase);

builder.Services
    .AddHttpClient<MenuApiClient>(c => c.BaseAddress = apiBase)
    .AddHttpMessageHandler<AuthHttpMessageHandler>();

builder.Services
    .AddHttpClient<OrderApiClient>(c => c.BaseAddress = apiBase)
    .AddHttpMessageHandler<AuthHttpMessageHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
