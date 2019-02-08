#if NET462
using System;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.ProviderUrlHelper;
using SFA.DAS.ProviderUrlHelper.Framework;


namespace SFA.DAS.ProviderUrlHelperTests.Framework
{
    public class UrlHelperExtensionsTestsFixtures
    {
        public UrlHelperExtensionsTestsFixtures()
        {
            DependencyResolverMock = new Mock<IDependencyResolver>();
            AutoConfigurationServiceMock = new Mock<IAutoConfigurationService>();
            DependencyResolverMock.Setup(sp => sp.GetService(typeof(IAutoConfigurationService))).Returns(AutoConfigurationService);

            ProviderUrlConfiguration = new ProviderUrlConfiguration();

            AutoConfigurationServiceMock.Setup(acs => acs.Get<ProviderUrlConfiguration>()).Returns(ProviderUrlConfiguration);

            System.Web.Mvc.DependencyResolver.SetResolver(DependencyResolver);
        }

        public ProviderUrlConfiguration ProviderUrlConfiguration { get; }
        
        public Mock<IDependencyResolver> DependencyResolverMock { get; }
        public IDependencyResolver DependencyResolver => DependencyResolverMock.Object;

        public Mock<IAutoConfigurationService> AutoConfigurationServiceMock { get; }
        public IAutoConfigurationService AutoConfigurationService => AutoConfigurationServiceMock.Object;
        

        public UrlHelperExtensionsTestsFixtures WithProviderApprenticeshipServiceBaseUrl(string baseUrl)
        {
            ProviderUrlConfiguration.ProviderApprenticeshipServiceBaseUrl = baseUrl;
            return this;
        }

        public string GetProviderfApprenticeshipServiceLink(string path)
        {
            return ProviderUrlHelperExtensions.ProviderApprenticeshipServiceLink(new UrlHelper(), path);
        }
    }
}

#endif