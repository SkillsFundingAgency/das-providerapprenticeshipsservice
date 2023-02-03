using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.NLog.Logger.Web;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, StructureMap.IContainer container)
        {
            filters.Add(new InvalidStateExceptionFilter());
            filters.Add(new ProviderUkPrnCheckActionFilter());
            filters.Add(new RequestIdActionFilter());
            filters.Add(new SessionIdActionFilter(HttpContextHelper.Current));
            filters.AddAuthorizationFilter();
            filters.Add(new RoatpCourseManagementCheckActionFilter(container.GetInstance<IGetRoatpBetaProviderService>()));
        }
    }
}