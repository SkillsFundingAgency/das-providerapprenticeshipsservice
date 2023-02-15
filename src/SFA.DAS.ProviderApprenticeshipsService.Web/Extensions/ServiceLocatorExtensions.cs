using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class ServiceLocator
    {
        public static T Get<T>(HttpContext httpContext) where T : class
        {
            return httpContext.RequestServices.GetService(typeof(T)) as T;
        }
    }
}