using System.Threading.Tasks;
using Refit;
using SFA.DAS.PAS.Jobs.ApiModels;

namespace SFA.DAS.PAS.Jobs.ApiClients;

public interface IRoatpApiClient
{
    [Get("/Organisations")]
    Task<GetAllProvidersResponse> GetProviders();
}
