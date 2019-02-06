using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Web;
using System.Web.Mvc;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Helpers
{
    public static class HtmlExtensions
    {
        public static bool IsManageReservationsEnabled(this HtmlHelper htmlHelper, long providerId)
        {
            var service = DependencyResolver.Current.GetService<IFeatureToggleService>();
            var isEnabled = service.Get<ManageReservations>().FeatureEnabled;
            return isEnabled;
        }

        public static async Task<bool> IsCreateCohortAuthorised(this HtmlHelper htmlHelper)
        {
            var httpContext = DependencyResolver.Current.GetService<HttpContextBase>();
            var ukprn = httpContext?.User?.Identity?.GetClaim("http://schemas.portal.com/ukprn");

            if (ukprn is null)
            {
                return false;
            }

            var mediator = DependencyResolver.Current.GetService<IMediator>();
            var response = await mediator.Send(new GetProviderHasRelationshipWithPermissionQueryRequest
            {
                Permission = Operation.CreateCohort,
                ProviderId = long.Parse(ukprn)
            });

            return response.HasPermission;
        }
    }
}