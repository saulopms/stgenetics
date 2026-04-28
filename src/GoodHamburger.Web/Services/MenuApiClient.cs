using System.Net.Http.Headers;
using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Web.Services;

public sealed class MenuApiClient
{
    private readonly HttpClient _http;

    public MenuApiClient(HttpClient http, AuthStateService authState)
    {
        _http = http;
        if (authState.Token is not null)
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authState.Token);
    }

    public Task<List<MenuItemResponse>?> GetMenuAsync() =>
        _http.GetFromJsonAsync<List<MenuItemResponse>>("api/menu");
}
