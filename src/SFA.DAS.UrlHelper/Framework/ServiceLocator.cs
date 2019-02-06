using System.Web.Mvc;

namespace SFA.DAS.ProviderUrlHelper.Framework
{
    public static class ServiceLocator
    {
        public static T Get<T>() where T : class
        {
            return DependencyResolver.Current.GetService<T>();
        }
    }
}
