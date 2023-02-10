using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetStringAsync(string url)
        {
            var httpResponse = await _httpClient.GetAsync(url);

            if (!httpResponse.IsSuccessStatusCode)
                throw new CustomHttpRequestException
                {
                    StatusCode = httpResponse.StatusCode
                };

            var responseString = await httpResponse.Content.ReadAsStringAsync();

            return responseString;
        }
    }

    public class CustomHttpRequestException : HttpRequestException
    {
        public HttpStatusCode StatusCode { get; set; }
    }

}