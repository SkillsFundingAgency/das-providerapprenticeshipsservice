#if NET462

using UrlHelper = System.Web.Mvc.UrlHelper;

namespace SFA.DAS.ProviderUrlHelper.Framework
{
    public static class ProviderUrlHelperExtensions
    {
        public static string ProviderCommitmentsLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();
            
            return linkGenerator.ProviderCommitmentsLink(path);
        }

        public static string ProviderApprenticeshipServiceLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();

            return linkGenerator.ProviderApprenticeshipServiceLink(path);
        }

        public static string ReservationsLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();

            return linkGenerator.ReservationsLink(path);
        }

        public static string RecruitLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();

            return linkGenerator.RecruitLink(path);
        }

        public static string TraineeshipLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();

            return linkGenerator.TraineeshipLink(path);
        }

        public static string RegistrationLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();

            return linkGenerator.RegistrationLink(path);
        }

        public static string EmployerDemandLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();
            return linkGenerator.EmployerDemandLink(path);
        }

        public static string CourseManagementLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();
            return linkGenerator.CourseManagementLink(path);
        }

        public static string ProviderFundingLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();
            return linkGenerator.ProviderFundingLink(path);
		}
		
        public static string APIManagementLink(this UrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator();
            return linkGenerator.APIManagementLink(path);
        }

        private static ILinkGenerator GetLinkGenerator()
        {
            var linkGenerator = ServiceLocator.Get<ILinkGenerator>();

            return linkGenerator;
        }
    }
}
#endif
