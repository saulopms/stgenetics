using System.Net.Http.Headers;

namespace GoodHamburger.Web.Services;

public sealed class AuthHttpMessageHandler(AuthStateService authState) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (authState.Token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authState.Token);

        return base.SendAsync(request, cancellationToken);
    }
}
