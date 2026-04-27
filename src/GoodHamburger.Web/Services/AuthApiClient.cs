using System.Net.Http.Json;

namespace GoodHamburger.Web.Services;

public sealed class AuthApiClient(HttpClient http)
{
    public async Task<string?> LoginAsync(string username, string password)
    {
        var response = await http.PostAsJsonAsync("api/auth/login",
            new { Username = username, Password = password });

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<TokenResult>();
        return result?.Token;
    }

    private sealed record TokenResult(string Token);
}
