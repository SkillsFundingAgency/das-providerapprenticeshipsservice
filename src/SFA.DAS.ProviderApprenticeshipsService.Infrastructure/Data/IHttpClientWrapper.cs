using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public interface IHttpClientWrapper
    {
        Task<string> GetStringAsync(string url);
    }
}