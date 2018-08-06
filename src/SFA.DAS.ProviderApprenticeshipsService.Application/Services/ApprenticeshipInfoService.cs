using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;
using Framework = SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse.Framework;
using Provider = SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider.Provider;
using Standard = SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse.Standard;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services
{
    public class ApprenticeshipInfoService : IApprenticeshipInfoService
    {
        private const string StandardsKey = "Standards";
        private const string FrameworksKey = "Frameworks";

        private readonly ICache _cache;
        private readonly IApprenticeshipInfoServiceConfiguration _configuration;
        private readonly IApprenticeshipInfoServiceMapper _mapper;

        public ApprenticeshipInfoService(ICache cache, IApprenticeshipInfoServiceConfiguration configuration, IApprenticeshipInfoServiceMapper mapper)
        {
            _cache = cache;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<StandardsView> GetStandardsAsync(bool refreshCache = false)
        {
            if (! await _cache.ExistsAsync(StandardsKey) || refreshCache)
            {
                var api = new StandardApiClient(_configuration.BaseUrl);

                var standards = api.FindAll()
                    .Where(s => s.IsActiveStandard) //this is to be removed in future
                    .OrderBy(x => x.Title).ToList();

                await _cache.SetCustomValueAsync(StandardsKey, _mapper.MapFrom(standards));
            }

            return await _cache.GetCustomValueAsync<StandardsView>(StandardsKey);
        }

        public async Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false)
        {
            if (!await _cache.ExistsAsync(FrameworksKey) || refreshCache)
            {
                var api = new FrameworkApiClient(_configuration.BaseUrl);

                var frameworks = api.FindAll()
                    .Where(s => s.IsActiveFramework) //this is to be removed in future
                    .OrderBy(x => x.Title).ToList();

                await _cache.SetCustomValueAsync(FrameworksKey, _mapper.MapFrom(frameworks));
            }

            return await _cache.GetCustomValueAsync<FrameworksView>(FrameworksKey);
        }

        public ProvidersView GetProvider(long ukPrn)
        {
            var api = new Providers.Api.Client.ProviderApiClient(_configuration.BaseUrl);
            var providers = api.Get(ukPrn);           
            return _mapper.MapFrom(providers);
        }
    }
}