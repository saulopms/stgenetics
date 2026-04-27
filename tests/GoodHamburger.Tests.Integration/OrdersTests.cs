using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace GoodHamburger.Tests.Integration;

public sealed class OrdersTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public OrdersTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { Username = "admin", Password = "admin123" });
        var body = await response.Content.ReadFromJsonAsync<TokenDto>();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    // ── POST /api/orders ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateOrder_WithSandwichFriesSoda_Returns201WithTwentyPercentDiscount()
    {
        await AuthenticateAsync();
        var response = await _client.PostAsJsonAsync("/api/orders",
            new { MenuItemIds = new[] { 1, 4, 5 } });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order!.DiscountPercentage.Should().Be(0.20m);
        order.Total.Should().Be(order.Subtotal * 0.80m);
    }

    [Fact]
    public async Task CreateOrder_WithDuplicateItem_Returns400()
    {
        await AuthenticateAsync();
        var response = await _client.PostAsJsonAsync("/api/orders",
            new { MenuItemIds = new[] { 1, 1 } });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidMenuItemId_Returns400()
    {
        await AuthenticateAsync();
        var response = await _client.PostAsJsonAsync("/api/orders",
            new { MenuItemIds = new[] { 99 } });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithTwoSandwiches_Returns400()
    {
        await AuthenticateAsync();
        var response = await _client.PostAsJsonAsync("/api/orders",
            new { MenuItemIds = new[] { 1, 2 } });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── GET /api/orders ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllOrders_ReturnsOk()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/api/orders");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── GET /api/orders/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task GetOrderById_ExistingOrder_ReturnsOk()
    {
        await AuthenticateAsync();
        var created  = await CreateOrderAsync([1, 4]);
        var response = await _client.GetAsync($"/api/orders/{created.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOrderById_NonExisting_Returns404()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync($"/api/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── PUT /api/orders/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task UpdateOrder_ChangingItems_RecalculatesDiscount()
    {
        await AuthenticateAsync();
        var created = await CreateOrderAsync([1]);

        var response = await _client.PutAsJsonAsync($"/api/orders/{created.Id}",
            new { MenuItemIds = new[] { 1, 5 } });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<OrderDto>();
        updated!.DiscountPercentage.Should().Be(0.15m);
    }

    [Fact]
    public async Task UpdateOrder_NonExisting_Returns404()
    {
        await AuthenticateAsync();
        var response = await _client.PutAsJsonAsync($"/api/orders/{Guid.NewGuid()}",
            new { MenuItemIds = new[] { 1 } });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── DELETE /api/orders/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task DeleteOrder_ExistingOrder_Returns204()
    {
        await AuthenticateAsync();
        var created  = await CreateOrderAsync([1]);
        var response = await _client.DeleteAsync($"/api/orders/{created.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrder_NonExisting_Returns404()
    {
        await AuthenticateAsync();
        var response = await _client.DeleteAsync($"/api/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Unauthenticated access ────────────────────────────────────────────────

    [Fact]
    public async Task GetOrders_WithoutToken_Returns401()
    {
        // AuthenticateAsync not called — no Bearer header
        var response = await _client.GetAsync("/api/orders");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<OrderDto> CreateOrderAsync(int[] ids)
    {
        var response = await _client.PostAsJsonAsync("/api/orders", new { MenuItemIds = ids });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OrderDto>())!;
    }

    private sealed record TokenDto(string Token);
    private sealed record OrderDto(Guid Id, decimal Subtotal, decimal DiscountPercentage, decimal Total);
}
