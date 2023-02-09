using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services.GetRoatpBetaProviderService
{
    public class GetRoatpBetaProviderService : IGetRoatpBetaProviderService
    {
        private const string CourseManagement = "CourseManagement";
        private readonly RoatpCourseManagementWebConfiguration _roatpCourseManagementWebConfiguration;

        public GetRoatpBetaProviderService(RoatpCourseManagementWebConfiguration roatpCourseManagementWebConfiguration)
        {
            _roatpCourseManagementWebConfiguration = roatpCourseManagementWebConfiguration;
        }

        public bool IsUkprnEnabled(int ukprn)
        {
            var featureToggles = _roatpCourseManagementWebConfiguration.ProviderFeaturesConfiguration.FeatureToggles;

            var courseManagementFeature = featureToggles.FirstOrDefault(f => f.Feature == CourseManagement);
            if (courseManagementFeature == null) return false;

            var providerUkrpns = !courseManagementFeature.IsEnabled || courseManagementFeature?.Whitelist == null ?
                        new List<int>() :
                        courseManagementFeature.Whitelist.Select(w => w.Ukprn).ToList();

            return providerUkrpns.Any(x => x == ukprn);
        }
    }
}