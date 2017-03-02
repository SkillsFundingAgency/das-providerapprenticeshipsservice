using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using Framework = SFA.DAS.ProviderApprenticeshipsService.Domain.Framework;
using Provider = SFA.DAS.ProviderApprenticeshipsService.Domain.Provider;

namespace SFA.DAS.ProviderApprenticeshipsService.Application
{
    public class ApprenticeshipInfoServiceWrapper : IApprenticeshipInfoServiceWrapper
    {
        private const string StandardsKey = "Standards";
        private const string FrameworksKey = "Frameworks";

        private readonly ICache _cache;
        private readonly IApprenticeshipInfoServiceConfiguration _configuration;
        private readonly ICurrentDateTime _currentDateTime;

        public ApprenticeshipInfoServiceWrapper(ICache cache, IApprenticeshipInfoServiceConfiguration configuration, ICurrentDateTime currentDateTime)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (currentDateTime == null)
                throw new ArgumentNullException(nameof(currentDateTime));

            _cache = cache;
            _configuration = configuration;
            _currentDateTime = currentDateTime;
        }

        public async Task<StandardsView> GetStandardsAsync(bool refreshCache = false)
        {
            if (! await _cache.ExistsAsync(StandardsKey) || refreshCache)
            {
                var api = new StandardApiClient(_configuration.BaseUrl);

                var standards = api.FindAll().OrderBy(x => x.Title).ToList();

                await _cache.SetCustomValueAsync(StandardsKey, MapFrom(standards));
            }

            return await _cache.GetCustomValueAsync<StandardsView>(StandardsKey);
        }

        public async Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false)
        {
            if (!await _cache.ExistsAsync(FrameworksKey) || refreshCache)
            {
                var api = new FrameworkApiClient(_configuration.BaseUrl);

                var frameworks = api.FindAll().OrderBy(x => x.Title).ToList();

                await _cache.SetCustomValueAsync(FrameworksKey, MapFrom(frameworks));
            }

            return await _cache.GetCustomValueAsync<FrameworksView>(FrameworksKey);
        }

        public ProvidersView GetProvider(long ukPrn)
        {
            var api = new Providers.Api.Client.ProviderApiClient(_configuration.BaseUrl);
            var providers = api.Get(ukPrn);           
            return MapFrom(providers);
        }

        private FrameworksView MapFrom(List<FrameworkSummary> frameworks)
        {
            return new FrameworksView
            {
                CreatedDate = _currentDateTime.Now,
                Frameworks = frameworks.Select(x => new Framework
                {
                    Id = x.Id,
                    Title = GetTitle(x.FrameworkName.Trim() == x.PathwayName.Trim() ? x.FrameworkName : x.Title, x.Level),
                    FrameworkCode = x.FrameworkCode,
                    FrameworkName = x.FrameworkName,
                    Level = x.Level,
                    PathwayCode = x.PathwayCode,
                    PathwayName = x.PathwayName,
                    Duration = new Duration
                    {
                        From = x.TypicalLength.From,
                        To = x.TypicalLength.To,
                        Unit = x.TypicalLength.Unit
                    },
                    MaxFunding = x.MaxFunding
                }).ToList()
            };
        }

        private ProvidersView MapFrom(Apprenticeships.Api.Types.Providers.Provider provider)
        {
            return new ProvidersView
            {
                CreatedDate = _currentDateTime.Now,
                Provider = new Provider()
                {
                    Ukprn = provider.Ukprn,
                    ProviderName = provider.ProviderName,
                    Email = provider.Email,
                    Phone = provider.Phone,
                    NationalProvider = provider.NationalProvider
                }
            };
        }

        private StandardsView MapFrom(List<StandardSummary> standards)
        {
            return new StandardsView
            {
                CreationDate = _currentDateTime.Now,
                Standards = standards.Select(x => new Domain.Standard
                {
                    Id = x.Id,
                    Level = x.Level,
                    Title = GetTitle(x.Title, x.Level),
                    Duration = new Duration
                    {
                        From = x.TypicalLength.From,
                        To = x.TypicalLength.To,
                        Unit = x.TypicalLength.Unit
                    },
                    MaxFunding = x.MaxFunding
                }).ToList()
            };
        }

        private static string GetTitle(string title, int level)
        {
            return $"{title}, Level: {level}";
        }
    }
}