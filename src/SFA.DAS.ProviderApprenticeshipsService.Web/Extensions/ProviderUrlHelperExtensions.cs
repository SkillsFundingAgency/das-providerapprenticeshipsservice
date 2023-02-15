using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.LinkGeneratorService;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class ProviderUrlHelperExtensions
    {
        public static string ProviderCommitmentsLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ProviderCommitmentsLink(path);
        }

        public static string ProviderApprenticeshipServiceLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ProviderApprenticeshipServiceLink(path);

        }

        public static string ReservationsLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ReservationsLink(path);
        }

        public static string RecruitLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.RecruitLink(path);
        }

        public static string TraineeshipLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.TraineeshipLink(path);
        }

        public static string RegistrationLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.RegistrationLink(path);
        }

        public static string EmployerDemandLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);
            return linkGenerator.EmployerDemandLink(path);
        }

        public static string CourseManagementLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);
            return linkGenerator.CourseManagementLink(path);
        }

        public static string ProviderFundingLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);
            return linkGenerator.ProviderFundingLink(path);
        }

        public static string APIManagementLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);
            return linkGenerator.APIManagementLink(path);
        }


        private static ILinkGeneratorService GetLinkGenerator(HttpContext httpContext)
        {
            return ServiceLocator.Get<ILinkGeneratorService>(httpContext);
        }
    }
}