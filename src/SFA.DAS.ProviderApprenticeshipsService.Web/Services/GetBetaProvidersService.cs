using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Services
{
    public class GetRoatpBetaProvidersService: IGetRoatpBetaProviderService
    {
        private const string CourseManagement = "CourseManagement";
        private readonly ProviderFeaturesConfiguration _providerFeaturesConfiguration;

        public GetRoatpBetaProvidersService(ProviderFeaturesConfiguration providerFeaturesConfiguration)
        {
            _providerFeaturesConfiguration = providerFeaturesConfiguration;
        }


        private List<int> GetBetaProviderUkprns()
        {
            var featureToggles = _providerFeaturesConfiguration.FeatureToggles;

            var courseManagementFeature = featureToggles.First(f => f.Feature == CourseManagement);
            return courseManagementFeature?.Whitelist == null ?
                new List<int>() :
                courseManagementFeature.Whitelist.Select(w => Convert.ToInt32(w.Ukprn)).ToList();

        }

        public bool IsUkprnEnabled(int ukprn)
        {
            return GetBetaProviderUkprns().Any(x => x == ukprn);
        }
    }
}