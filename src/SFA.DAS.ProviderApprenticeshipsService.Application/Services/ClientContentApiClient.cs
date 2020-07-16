using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Threading.Tasks;
using System.Net.Http;
using SFA.DAS.Authentication.Extensions.Legacy;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services
{
    public class ClientContentApiClient : ApiClientBase, IClientContentApiClient
    {
        private readonly string ApiBaseUrl;

        public ClientContentApiClient(HttpClient client, IContentClientApiConfiguration configuration) : base(client)
        {
            ApiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
                ? configuration.ApiBaseUrl
                : configuration.ApiBaseUrl + "/";
        }

        public async Task<string> Get(string type, string applicationId)
        {   
            var uri = $"{ApiBaseUrl}api/content?applicationId={applicationId}&type={type}";
            string content = await GetAsync(uri);            
            return content;
        }
    }
}
    