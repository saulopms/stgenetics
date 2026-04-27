using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace GoodHamburger.Web.Services;

public sealed class CustomAuthStateProvider(AuthStateService authState) : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!authState.IsAuthenticated)
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, authState.Username ?? "user")],
            authenticationType: "jwt");

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public void NotifyStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
