using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;

public class ContractFeedProcessorHttpClient : IContractFeedProcessorHttpClient
{
    private readonly AzureAuthentication _authenticationCredentials;
    private static string VendorAtomMediaType = "application/vnd.sfa.contract.v1+atom+xml";
    
    private static readonly HttpClient HttpClient = InitHttpClient();

    public string BaseAddress { get; }

    private static HttpClient InitHttpClient()
    {
        var client  = new HttpClient();

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(VendorAtomMediaType));

        return client;
    }
    
    public ContractFeedProcessorHttpClient(IContractFeedConfiguration config)
    {
        _authenticationCredentials = new AzureAuthentication(config.AADInstance, config.Tenant, config.ClientId, config.ResourceId, config.ClientSecret);
        BaseAddress = config.BaseAddress;
    }
    
    public virtual HttpClient GetAuthorizedHttpClient()
    {
        var token = _authenticationCredentials.GetAuthenticationResult().Result.AccessToken;
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return HttpClient;
    }
}