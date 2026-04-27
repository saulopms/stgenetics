namespace GoodHamburger.Web.Services;

public sealed class AuthStateService
{
    public string? Token    { get; private set; }
    public string? Username { get; private set; }
    public bool IsAuthenticated => Token is not null;

    public event Action? OnChange;

    public void SetUser(string token, string username)
    {
        Token    = token;
        Username = username;
        OnChange?.Invoke();
    }

    public void Clear()
    {
        Token    = null;
        Username = null;
        OnChange?.Invoke();
    }
}
