using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests
{
    public class TestServiceLocator : ServiceLocatorImplBase
    {
        public TestServiceLocator(IContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IContainer Container { get; set; }

        public void Dispose()
        {
            Container.Dispose();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return DoGetAllInstances(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return Container.GetAllInstances(serviceType).Cast<object>();
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            IContainer container = Container;

            if (string.IsNullOrEmpty(key))
            {
                return serviceType.IsAbstract || serviceType.IsInterface
                    ? container.TryGetInstance(serviceType)
                    : container.GetInstance(serviceType);
            }

            return container.GetInstance(serviceType, key);
        }
    }
}