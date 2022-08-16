using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Services
{
    public class GetRoatpBetaProvidersService: IGetRoatpBetaProviderService
    {
        private const string CourseManagement = "CourseManagement";
        private readonly RoatpCourseManagementCourseManagementWebConfiguration _roatpCourseManagementCourseManagementWebConfiguration;

        public GetRoatpBetaProvidersService(RoatpCourseManagementCourseManagementWebConfiguration roatpCourseManagementCourseManagementWebConfiguration)
        {
            _roatpCourseManagementCourseManagementWebConfiguration = roatpCourseManagementCourseManagementWebConfiguration;
        }

        public bool IsUkprnEnabled(int ukprn)
        {
            var featureToggles = _roatpCourseManagementCourseManagementWebConfiguration.ProviderFeaturesConfiguration.FeatureToggles;

            var courseManagementFeature = featureToggles.First(f => f.Feature == CourseManagement);
            var providerUkrpns= !courseManagementFeature.IsEnabled 
                                        || courseManagementFeature?.Whitelist == null ?
                        new List<int>() :
                        courseManagementFeature.Whitelist.Select(w => w.Ukprn).ToList();

            return providerUkrpns.Any(x => x == ukprn);
        }
    }
}