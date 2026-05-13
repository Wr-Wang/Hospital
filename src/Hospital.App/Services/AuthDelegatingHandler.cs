using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.App.Services;

public sealed class AuthDelegatingHandler : DelegatingHandler
{
    private readonly IAppContext _appContext;

    public AuthDelegatingHandler(IAppContext appContext)
    {
        _appContext = appContext;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_appContext.AccessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _appContext.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
