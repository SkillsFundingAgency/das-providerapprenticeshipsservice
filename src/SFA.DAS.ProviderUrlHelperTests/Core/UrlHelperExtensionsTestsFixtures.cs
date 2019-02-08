#if NETCOREAPP
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.ProviderUrlHelper;
using SFA.DAS.ProviderUrlHelper.Core;


namespace SFA.DAS.ProviderUrlHelperTests.Core
{
    public class UrlHelperExtensionsTestsFixtures
    {
        public UrlHelperExtensionsTestsFixtures()
        {
            HttpContextMock = new Mock<HttpContext>();
            ServiceProviderMock = new Mock<IServiceProvider>();
            AutoConfigurationServiceMock = new Mock<IAutoConfigurationService>();

            HttpContextMock.Setup(hcm => hcm.RequestServices).Returns(ServiceProvider);
            ServiceProviderMock.Setup(sp => sp.GetService(typeof(IAutoConfigurationService))).Returns(AutoConfigurationService);

            ProviderUrlConfiguration = new ProviderUrlConfiguration();

            AutoConfigurationServiceMock.Setup(acs => acs.Get<ProviderUrlConfiguration>()).Returns(ProviderUrlConfiguration);

            UrlActionContext = new UrlActionContext();
            UrlRouteContext =  new UrlRouteContext();
            ActionContext =  new ActionContext(HttpContext, new RouteData(), new ActionDescriptor());

            HelperBase = new UrlHelperMock(ActionContext);
        }

        public UrlHelperBase HelperBase { get; }

        public ProviderUrlConfiguration ProviderUrlConfiguration { get; }
        
        public Mock<HttpContext> HttpContextMock { get; }
        public HttpContext HttpContext => HttpContextMock.Object;

        public Mock<IServiceProvider> ServiceProviderMock { get; }
        public IServiceProvider ServiceProvider => ServiceProviderMock.Object;

        public Mock<IAutoConfigurationService> AutoConfigurationServiceMock { get; }
        public IAutoConfigurationService AutoConfigurationService => AutoConfigurationServiceMock.Object;
        

        public ActionContext ActionContext { get; }
        public UrlActionContext UrlActionContext { get; }
        public UrlRouteContext UrlRouteContext { get; }

        public UrlHelperExtensionsTestsFixtures WithProviderApprenticeshipServiceBaseUrl(string baseUrl)
        {
            ProviderUrlConfiguration.ProviderApprenticeshipServiceBaseUrl = baseUrl;
            return this;
        }

        public string GetProviderfApprenticeshipServiceLink(string path)
        {
            return ProviderUrlHelperExtensions.ProviderApprenticeshipServiceLink(HelperBase, path);
        }

        class UrlHelperMock : UrlHelperBase
        {
            public UrlHelperMock(ActionContext actionContext) : base(actionContext)
            {
            }

            public override string Action(UrlActionContext actionContext)
            {
                return string.Empty;
            }

            public override string RouteUrl(UrlRouteContext routeContext)
            {
                return string.Empty;
            }
        }
    }
}

#endif