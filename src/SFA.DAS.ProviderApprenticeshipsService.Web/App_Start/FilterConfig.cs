using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, StructureMap.IContainer container)
        {
            filters.Add(new RequestIdActionFilter()); // TO BE REPLACED
            filters.Add(new SessionIdActionFilter(HttpContextHelper.Current)); // TO BE REPLACED
        }
    }
}