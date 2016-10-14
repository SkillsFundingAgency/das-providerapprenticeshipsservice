using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using Provider = SFA.DAS.ProviderApprenticeshipsService.Domain.Provider;
using StandardSummary = SFA.DAS.Apprenticeships.Api.Client.Models.StandardSummary;

namespace SFA.DAS.ProviderApprenticeshipsService.Application
{
    public class ApprenticeshipInfoServiceWrapper : IApprenticeshipInfoServiceWrapper
    {
        private readonly ICache _cache;
        private readonly IApprenticeshipInfoServiceConfiguration _configuration;

        public ApprenticeshipInfoServiceWrapper(ICache cache, IApprenticeshipInfoServiceConfiguration configuration)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<StandardsView> GetStandardsAsync(string key, bool refreshCache = false)
        {
            if (! await _cache.ExistsAsync(key) || refreshCache)
            {
                var api = new StandardApiClient(_configuration.BaseUrl);

                var standards = api.FindAll().ToList();

                await _cache.SetCustomValueAsync(key, MapFrom(standards));
            }

            return await _cache.GetCustomValueAsync<StandardsView>(key);
        }

        public ProvidersView GetProvider(int ukPrn)
        {
            var api = new ProviderApiClient(_configuration.BaseUrl);

            return MapFrom(api.Get(ukPrn));
        }

        private static ProvidersView MapFrom(IEnumerable<Sfa.Das.ApprenticeshipInfoService.Core.Models.Provider> providers)
        {
            return new ProvidersView
            {
                CreatedDate = DateTime.UtcNow,
                Providers = providers.Select(x => new Provider
                {
                    Ukprn = x.Ukprn,
                    ProviderName = x.ProviderName,
                    Email = x.Email,
                    Phone = x.Phone,
                    NationalProvider = x.NationalProvider
                }).ToList()
            };
        }

        private static StandardsView MapFrom(List<StandardSummary> standards)
        {
            return new StandardsView
            {
                CreationDate = DateTime.UtcNow,
                Standards = standards.Select(x => new Domain.Standard
                {
                    Id = x.Id,
                    Level = x.Level,
                    Title = x.Title,
                    Duration = new Duration
                    {
                        From = x.TypicalLength.From,
                        To = x.TypicalLength.To,
                        Unit = x.TypicalLength.Unit
                    }
                }).ToList()
            };
        }
    }
}