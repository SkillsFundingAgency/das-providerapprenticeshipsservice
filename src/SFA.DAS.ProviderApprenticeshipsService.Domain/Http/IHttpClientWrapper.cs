using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Http
{
    public interface IHttpClientWrapper
    {
        Task<string> SendMessage<T>(T content, string url);
        Task<T> Get<T>(string authToken, string url);
        Task<string> GetString(string url, string accessToken);
        string BaseUrl { get; set; }
        List<MediaTypeWithQualityHeaderValue> MediaTypeWithQualityHeaderValueList { get; set; }
        string AuthScheme { get; set; }
    }
}
