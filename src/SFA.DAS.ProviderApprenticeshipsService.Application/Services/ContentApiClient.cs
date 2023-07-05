using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Threading.Tasks;
using System.Net.Http;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services;

public class ContentApiClient : ApiClientBase, IContentApiClient
{
    private readonly string _apiBaseUrl;
   
    public ContentApiClient(HttpClient client, IManagedIdentityClientConfiguration configuration) : base(client)
    {
        _apiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
            ? configuration.ApiBaseUrl
            : configuration.ApiBaseUrl + "/";
    }
    public async Task<string> Get(string type, string applicationId)
    {   
        var uri = $"{_apiBaseUrl}api/content?applicationId={applicationId}&type={type}";
        return await GetAsync(uri);
    }
}