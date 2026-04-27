using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Web.Services;

public sealed class MenuApiClient(HttpClient http)
{
    public Task<List<MenuItemResponse>?> GetMenuAsync() =>
        http.GetFromJsonAsync<List<MenuItemResponse>>("api/menu");
}
