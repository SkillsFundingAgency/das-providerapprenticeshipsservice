using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

public interface IHttpClientWrapper
{
    Task<string> GetStringAsync(string url);
}