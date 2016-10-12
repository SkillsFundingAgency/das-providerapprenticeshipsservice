using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Client.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application
{
    public class ApprenticeshipInfoServiceWrapper
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

        public StandardsView GetStandards(string key, bool refreshCache = false)
        {
            if (!_cache.Exists(key) || refreshCache)
            {
                Console.WriteLine("Refreshing cache");

                var api = new StandardApiClient(_configuration.BaseUrl);

                var standards = api.FindAll().ToList();

                _cache.SetCustomValue(key, MapFrom(standards));
            }

            return _cache.GetCustomValue<StandardsView>(key);
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