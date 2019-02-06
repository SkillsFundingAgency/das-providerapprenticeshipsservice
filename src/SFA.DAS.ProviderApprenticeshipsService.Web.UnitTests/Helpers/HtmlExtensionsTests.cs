using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using StructureMap;
using System.Web.Mvc;
using FeatureToggle;
using MediatR;
using Moq;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Web.Helpers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Helpers
{
    [TestFixture]
    public class HtmlExtensionsTests
    {
        [TestFixture]
        public class WhenCallingIsManageReservationsEnabled
        {
            public Mock<IFeatureToggleService> FeatureToggleServiceMock;
            public Mock<IFeatureToggle> FeatureToggleMock;
            public TestServiceLocator TestServiceLocator;

            [SetUp]
            public void SetUp()
            {
                FeatureToggleMock = new Mock<IFeatureToggle>();
                FeatureToggleServiceMock = new Mock<IFeatureToggleService>();
                FeatureToggleServiceMock.Setup(x => x.Get<ManageReservations>()).Returns(FeatureToggleMock.Object);

                IContainer container = new Container(c =>
                {
                    c.For<IFeatureToggleService>().Use(FeatureToggleServiceMock.Object);
                });

                TestServiceLocator = new TestServiceLocator(container);
                DependencyResolver.SetResolver(TestServiceLocator);
            }

            [TestCase(true)]
            [TestCase(false)]
            public void ThenReturnsEnabled(bool expected)
            {
                FeatureToggleMock.Setup(x => x.FeatureEnabled).Returns(expected);
                var result = HtmlExtensions.IsManageReservationsEnabled(null, 1);
                Assert.AreEqual(result, expected);
            }

            [TearDown]
            public void TearDown()
            {
                if(TestServiceLocator != null)
                    TestServiceLocator.Dispose();
            }
        }

        [TestFixture]
        public class WhenCallingIsCreateCohortAuthorised
        {
            public HttpContextBase FakeHttpContextBase;
            public Mock<IMediator> MockMediator;
            public TestServiceLocator TestServiceLocator;

            [SetUp]
            public void SetUp()
            {
                FakeHttpContextBase = new FakeHttpContext();
                MockMediator = new Mock<IMediator>();

                IContainer container = new Container(c =>
                {
                    c.For<HttpContextBase>().Use(FakeHttpContextBase);
                    c.For<IMediator>().Use(MockMediator.Object);
                });

                TestServiceLocator = new TestServiceLocator(container);
                DependencyResolver.SetResolver(TestServiceLocator);
            }

            [TestCase(true)]
            [TestCase(false)]
            public async Task ThenReturnsThePermission(bool expected)
            {
                FakeHttpContextBase.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("http://schemas.portal.com/ukprn", "1234", "string") }));
                MockMediator
                    .Setup(x => x.Send(It.IsAny<GetProviderHasRelationshipWithPermissionQueryRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetProviderHasRelationshipWithPermissionQueryResponse
                    {
                        HasPermission = expected
                    });
                var result = await HtmlExtensions.IsCreateCohortAuthorised(null);
                Assert.AreEqual(result, expected);
            }

            [Test]
            public async Task ThenReturnsFalseIfNoUserLoggedIn()
            {
                FakeHttpContextBase.User = null;
                var result = await HtmlExtensions.IsCreateCohortAuthorised(null);
                Assert.IsFalse(result);
            }

            [TearDown]
            public void TearDown()
            {
                if (TestServiceLocator != null)
                    TestServiceLocator.Dispose();
            }
        }
    }
}