using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

public interface ICommitmentsV2ApiClient
{
    Task<GetCohortResponse> GetCohort(long cohortId);
    Task<bool> ApprenticeEmailRequired(long providerId);
    Task<bool> OptionalEmail(long providerId, long employerId);
    Task<GetAllProvidersResponse> GetProviders();
}