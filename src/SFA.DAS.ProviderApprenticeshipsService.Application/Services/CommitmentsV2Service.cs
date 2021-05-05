using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services
{
    public class CommitmentsV2Service : ICommitmentsV2Service
    {
        private readonly ICommitmentsV2ApiClient _commitmentsV2ApiClient;

        public CommitmentsV2Service(ICommitmentsV2ApiClient commitmentsV2ApiClient)
        {
            _commitmentsV2ApiClient = commitmentsV2ApiClient;
        }
        public async Task<bool> CohortIsCompleteForProvider(long cohortId)
        {
            var cohort = await _commitmentsV2ApiClient.GetCohort(cohortId);
            return cohort.IsCompleteForProvider;
        }
    }
}
