using System;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services
{
    public class ApprenticeshipInfoService : IApprenticeshipInfoService
    {
        private readonly ICommitmentsV2ApiClient _commitmentsV2Api;
        
        public ApprenticeshipInfoService(ICommitmentsV2ApiClient commitmentsV2Api)
        {
            _commitmentsV2Api = commitmentsV2Api;
        }

        public async Task<ProvidersView> GetProvider(long ukprn)
        {
            try
            {
                var api = await _commitmentsV2Api.GetProvider(ukprn);
                return new ProvidersView
                {
                    CreatedDate = DateTime.UtcNow,
                    Provider = new Provider
                    {
                        Ukprn = api.ProviderId,
                        ProviderName = api.Name
                    }
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}