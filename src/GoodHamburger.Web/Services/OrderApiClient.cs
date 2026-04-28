using System.Net.Http.Headers;
using System.Net.Http.Json;
using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Web.Services;

public sealed class OrderApiClient
{
    private readonly HttpClient _http;

    public OrderApiClient(HttpClient http, AuthStateService authState)
    {
        _http = http;
        if (authState.Token is not null)
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authState.Token);
    }

    public Task<List<OrderResponse>?> GetAllAsync() =>
        _http.GetFromJsonAsync<List<OrderResponse>>("api/orders");

    public Task<OrderResponse?> GetByIdAsync(Guid id) =>
        _http.GetFromJsonAsync<OrderResponse>($"api/orders/{id}");

    public async Task<OrderResponse?> CreateAsync(CreateOrderRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderResponse>();
    }

    public async Task<OrderResponse?> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/orders/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderResponse>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/orders/{id}");
        response.EnsureSuccessStatusCode();
    }
}
