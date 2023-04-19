using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

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
        {
            throw new CustomHttpRequestException(httpResponse.StatusCode, httpResponse.ReasonPhrase);
        }

        return await httpResponse.Content.ReadAsStringAsync();
    }
}

public class CustomHttpRequestException : HttpRequestException
{
    public CustomHttpRequestException(HttpStatusCode statusCode, string reasonPhrase) 
        :base($"An unexpected HTTP error occurred due to reason '{reasonPhrase}'.", null,  statusCode) { }
}