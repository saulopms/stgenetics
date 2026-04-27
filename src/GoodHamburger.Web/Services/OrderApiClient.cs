using System.Net.Http.Json;
using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Web.Services;

public sealed class OrderApiClient(HttpClient http)
{
    public Task<List<OrderResponse>?> GetAllAsync() =>
        http.GetFromJsonAsync<List<OrderResponse>>("api/orders");

    public Task<OrderResponse?> GetByIdAsync(Guid id) =>
        http.GetFromJsonAsync<OrderResponse>($"api/orders/{id}");

    public async Task<OrderResponse?> CreateAsync(CreateOrderRequest request)
    {
        var response = await http.PostAsJsonAsync("api/orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderResponse>();
    }

    public async Task<OrderResponse?> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/orders/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderResponse>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await http.DeleteAsync($"api/orders/{id}");
        response.EnsureSuccessStatusCode();
    }
}
