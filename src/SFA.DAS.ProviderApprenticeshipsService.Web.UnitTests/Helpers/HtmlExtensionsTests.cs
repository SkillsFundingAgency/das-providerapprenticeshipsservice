using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using NUnit.Framework;
using StructureMap;
using System.Web.Mvc;
using FeatureToggle;
using Moq;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Web.Helpers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Helpers
{
    [TestFixture]
    public class HtmlExtensionsTests
    {
        [TestFixture]
        public class WhenCallingCanShowReservationsLink
        {
            public Mock<IFeatureToggleService> FeatureToggleServiceMock;
            public Mock<IFeatureToggle> FeatureToggleMock;
            public HttpContextBase FakeHttpContextBase;
            public TestServiceLocator TestServiceLocator;


            [SetUp]
            public void SetUp()
            {
                FakeHttpContextBase = new FakeHttpContext();

                FeatureToggleMock = new Mock<IFeatureToggle>();
                FeatureToggleServiceMock = new Mock<IFeatureToggleService>();
                FeatureToggleServiceMock.Setup(x => x.Get<ManageReservations>()).Returns(FeatureToggleMock.Object);


                IContainer container = new Container(c =>
                {
                    c.For<IFeatureToggleService>().Use(FeatureToggleServiceMock.Object);
                    c.For<HttpContextBase>().Use(FakeHttpContextBase);
                });

                TestServiceLocator = new TestServiceLocator(container);
                DependencyResolver.SetResolver(TestServiceLocator);
            }

            [TestCase(true, true)]
            [TestCase(false, false)]
            public void ThenReturnsTheExpectedPermission(bool userPermission, bool expectedResult)
            {
                //Following line resolves to true in all cases as it is lazy loaded, so parameterising it leads to failing tests
                FeatureToggleMock.Setup(x => x.FeatureEnabled).Returns(true);
                FakeHttpContextBase.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(DasClaimTypes.ShowReservations, userPermission.ToString(), "bool") }));
                var result = HtmlExtensions.CanShowReservationsLink(null);
                Assert.AreEqual(expectedResult, result);
            }

            [Test]
            public void ThenReturnsFalseIfNoUserLoggedIn()
            {
                FeatureToggleMock.Setup(x => x.FeatureEnabled).Returns(true);
                FakeHttpContextBase.User = null;
                var result = HtmlExtensions.CanShowReservationsLink(null);
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