using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.UserIdentityService;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        private Web.Controllers.HomeController _sut;
        private Mock<IAuthenticationOrchestrator> _mockAuthenticationOrchestrator;
        private ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;
        private Mock<IUserIdentityService> _userIdentityService;
        private Mock<IProviderCommitmentsLogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _userIdentityService = new Mock<IUserIdentityService>();
            _logger = new Mock<IProviderCommitmentsLogger>();
            _userIdentityService.Setup(x => x.UpsertUserIdentityAttributes(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(Unit.Value));
            _providerApprenticeshipsServiceConfiguration = new ProviderApprenticeshipsServiceConfiguration();
            _mockAuthenticationOrchestrator = new Mock<IAuthenticationOrchestrator>();
        }

        [Test]
        public void Index_When_DfESignIn_True_ShouldReturnHomeView()
        {
            //arrange
            _providerApprenticeshipsServiceConfiguration.UseDfESignIn = true;
            _sut = new Web.Controllers.HomeController(_providerApprenticeshipsServiceConfiguration, _mockAuthenticationOrchestrator.Object);

            // sut
            var result = _sut.Index();

            var vr = result as ViewResult;

            // assert
            vr.Should().NotBeNull();
        }

        [Test]
        public void Index_When_DfESignIn_False_ShouldRedirectToAccount()
        {
            //arrange

            _providerApprenticeshipsServiceConfiguration.UseDfESignIn = false;
            _sut = new Web.Controllers.HomeController(_providerApprenticeshipsServiceConfiguration, _mockAuthenticationOrchestrator.Object);

            // sut
            var result = _sut.Index();


            // assert
            var vr = result as RedirectToRouteResult;

            vr.Should().NotBeNull();
            vr.RouteName.Should().NotBeNullOrEmpty();
            vr.RouteName.Should().Be(RouteNames.AccountHome);
        }

        [Test, AutoData]
        public async Task SignIn_ShouldRedirectToAccount(
            string nameIdentifier,
            string name,
            string authType)
        {
            //arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new(ClaimTypes.NameIdentifier, nameIdentifier),
                new(ClaimTypes.Name, name)
            }, authType));

            _providerApprenticeshipsServiceConfiguration.UseDfESignIn = true;
            _sut = new Web.Controllers.HomeController(_providerApprenticeshipsServiceConfiguration, _mockAuthenticationOrchestrator.Object);
            _sut.ControllerContext.HttpContext = new DefaultHttpContext {User = user};

            // sut
            var result = await _sut.SignIn();

            var vr = result as RedirectToRouteResult;

            // assert
            vr.Should().NotBeNull();
            vr.RouteName.Should().NotBeNullOrEmpty();
            vr.RouteName.Should().Be(RouteNames.AccountHome);
            _mockAuthenticationOrchestrator.Verify(x => x.SaveIdentityAttributes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
