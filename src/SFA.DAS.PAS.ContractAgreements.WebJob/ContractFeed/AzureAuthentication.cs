using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;

public class AzureAuthentication
{
    private readonly string _aadInstance;
    private readonly string _tenant;
    private readonly string _clientId;
    private readonly string _resourceId;
    private IConfidentialClientApplication _app;

    public AzureAuthentication(string aadInstance, string tenant, string clientId, string resourceId)
    {
        _aadInstance = aadInstance;
        _tenant = tenant;
        _clientId = clientId;
        _resourceId = resourceId;
    }

    public async Task<AuthenticationResult> GetAuthenticationResult()
    {
        var authority = string.Format(_aadInstance, _tenant);

        if (_app == null)
        {
            _app = ConfidentialClientApplicationBuilder.Create(_clientId)
                //.WithClientSecret(clientSecret) // TODO
                .WithAuthority(authority)
                .Build();
        }

        AuthenticationResult authResult = null;
        var retryCount = 0;
        bool retry;

        do
        {
            retry = false;
            try
            {
                authResult = await _app.AcquireTokenForClient(
                        new[] { $"{_resourceId}/.default" })
                    .ExecuteAsync()
                    .ConfigureAwait(false);
                return authResult;
            }
            catch (MsalClientException ex)
            {
                if (ex.ErrorCode == "temporarily_unavailable")
                {
                    retry = true;
                    retryCount++;
                    Thread.Sleep(3000);
                }
            }
        } while (retry && retryCount < 3);

        if (authResult == null)
        {
            throw new AuthenticationException("Could not authenticate with the OAUTH2 claims provider after several attempts");
        }

        return authResult;
    }
}