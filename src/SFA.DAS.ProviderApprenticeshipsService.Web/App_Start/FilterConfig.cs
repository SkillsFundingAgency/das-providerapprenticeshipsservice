using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.NLog.Logger.Web;
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

            filters.Add(new RequestIdActionFilter());
            filters.Add(new SessionIdActionFilter(HttpContext.Current));

            if (!IsUsingStubProviderRelationshipsSetting())
            {
                filters.AddAuthorizationFilter();
            }            
        }

        private static bool IsUsingStubProviderRelationshipsSetting()
        {
            var value = CloudConfigurationManager.GetSetting("UseStubProviderRelationships");

            if (value == null)
            {
                return false;
            }

            return bool.TryParse(value, out var result) && result;
        }
    }
}