using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace GoodHamburger.Tests.Integration;

public sealed class MenuTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public MenuTests(ApiFactory factory)
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

    [Fact]
    public async Task GetMenu_ReturnsAllFiveItems()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/api/menu");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        items!.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetMenu_ContainsThreeSandwiches()
    {
        await AuthenticateAsync();
        var items = await _client.GetFromJsonAsync<List<MenuItemDto>>("/api/menu");
        items!.Count(i => i.Type == "Sandwich").Should().Be(3);
    }

    [Fact]
    public async Task GetMenu_ContainsFriesAndSoda()
    {
        await AuthenticateAsync();
        var items = await _client.GetFromJsonAsync<List<MenuItemDto>>("/api/menu");
        items!.Should().Contain(i => i.Type == "Fries");
        items!.Should().Contain(i => i.Type == "Soda");
    }

    [Fact]
    public async Task GetMenu_WithoutToken_Returns401()
    {
        // _client has no Authorization header in this instance (AuthenticateAsync not called)
        var response = await _client.GetAsync("/api/menu");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private sealed record TokenDto(string Token);
    private sealed record MenuItemDto(int Id, string Name, decimal Price, string Type);
}
