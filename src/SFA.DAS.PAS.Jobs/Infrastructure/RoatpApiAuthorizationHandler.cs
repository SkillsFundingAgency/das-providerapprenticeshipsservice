using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.PAS.Jobs.Configuration;

namespace SFA.DAS.PAS.Jobs.Infrastructure;

[ExcludeFromCodeCoverage]
internal sealed class RoatpApiAuthorizationHandler(
    IAzureClientCredentialHelper azureClientCredentialHelper,
    RoatpApiConfiguration config) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(config.IdentifierUri))
        {
            var accessToken = await azureClientCredentialHelper.GetAccessTokenAsync(config.IdentifierUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
