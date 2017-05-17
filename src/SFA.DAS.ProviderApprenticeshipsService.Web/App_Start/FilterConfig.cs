using System;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Not required - filters.Add(new HandleErrorAttribute()); 
            filters.Add(new InvalidStateExceptionFilter());
            filters.Add(new ProviderUkPrnCheckActionFilter());
            filters.Add(new DasRoleCheckActionFilter());
        }
    }
}
