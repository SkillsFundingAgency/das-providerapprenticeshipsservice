using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Helpers;
using StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Helpers
{
    public partial class HtmlExtensionsTests
    {
        [TestFixture]
        public class WhenCallingCanShowReservationsLink
        {
            public HttpContextBase FakeHttpContextBase;
            public TestServiceLocator TestServiceLocator;


            [SetUp]
            public void SetUp()
            {
                FakeHttpContextBase = new FakeHttpContext();

                IContainer container = new Container(c =>
                {
                    c.For<HttpContextBase>().Use(FakeHttpContextBase);
                });

                TestServiceLocator = new TestServiceLocator(container);
                DependencyResolver.SetResolver(TestServiceLocator);
            }

            [TestCase(true, true)]
            [TestCase(false, false)]
            public void ThenReturnsTheExpectedPermission(bool userPermission, bool expectedResult)
            {
                FakeHttpContextBase.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(DasClaimTypes.ShowReservations, userPermission.ToString(), "bool") }));
                var result = HtmlExtensions.CanShowReservationsLink(null);
                Assert.AreEqual(expectedResult, result);
            }

            [Test]
            public void ThenReturnsFalseIfNoUserLoggedIn()
            {
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