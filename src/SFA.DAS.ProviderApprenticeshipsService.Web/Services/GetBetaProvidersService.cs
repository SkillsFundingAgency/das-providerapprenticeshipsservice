using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Services
{
    public class GetRoatpBetaProviderService: IGetRoatpBetaProviderService
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

            var courseManagementFeature = featureToggles.First(f => f.Feature == CourseManagement);
            var providerUkrpns= !courseManagementFeature.IsEnabled 
                                        || courseManagementFeature?.Whitelist == null ?
                        new List<int>() :
                        courseManagementFeature.Whitelist.Select(w => w.Ukprn).ToList();

            return providerUkrpns.Any(x => x == ukprn);
        }
    }
}