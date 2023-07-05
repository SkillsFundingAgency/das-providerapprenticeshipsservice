using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

public interface IContentApiClient
{
    Task<string> Get(string type, string applicationId);
}