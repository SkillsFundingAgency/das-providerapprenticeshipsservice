using System;
using SFA.DAS.AutoConfiguration;
#if NETFRAMEWORK
using System.Web.Mvc;
using UrlHelper=System.Web.Mvc.UrlHelper;
#elif NETCOREAPP
using UrlHelper=Microsoft.AspNetCore.Mvc.Routing .UrlHelper;
using Microsoft.AspNetCore.Http;
#endif

namespace SFA.DAS.ProviderUrlHelper
{

#if NETSTANDARD
    public class UrlHelper
    {

    }

    public static class ServiceLocator
    {
        public static T Get<T>() where T : class
        {
            throw new NotImplementedException("ServiceLocator is not implemented in .Net Standard framework");
        }
    }

#elif NETFRAMEWORK

    public static class ServiceLocator
    {
        public static T Get<T>() where T : class
        {
            return DependencyResolver.Current.GetService<T>();
        }
    }

#elif NETCOREAPP

    public static class ServiceLocator
    {
        public static T Get<T>(HttpContext httpContext) where T : class
        {
            return httpContext.RequestServices.GetService(typeof(T)) as T;
        }
    }

#endif
}