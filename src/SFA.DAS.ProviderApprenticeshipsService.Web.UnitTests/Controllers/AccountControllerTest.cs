using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Account;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Controllers
{
    [TestFixture]
    public class AccountControllerTest
    {
        private AccountController _controller = null!;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _mockCookieStorageService = null!;
        private Mock<IConfiguration> _mockConfiguration = null!;
        private Mock<IAccountOrchestrator> _mockAccountOrchestrator = null!;


        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _mockAccountOrchestrator = new Mock<IAccountOrchestrator>();

        }


        [TestCase("test", "https://test-profile.signin.education.gov.uk/")]
        [TestCase("preprod", "https://pp-profile.signin.education.gov.uk/")]
        [TestCase("local", "https://test-profile.signin.education.gov.uk/")]
        [TestCase("prod", "https://profile.signin.education.gov.uk/")]
        [TestCase("prd", "https://profile.signin.education.gov.uk/")]
        public void When_ChangeOfDetails_Then_ViewIsReturned(string env, string profilePageLink)
        {
            //arrange
            _mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
            _controller = new AccountController(_mockAccountOrchestrator.Object, _mockCookieStorageService.Object, _mockConfiguration.Object, Mock.Of<ILogger<AccountController>>())
            {
                ControllerContext = new ControllerContext()
            };
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            //sut
            var result = (ViewResult)_controller.ChangeSignInDetails();

            //assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var actualModel = result.Model as ChangeOfDetailsViewModel;
                actualModel.Should().NotBeNull();
                actualModel?.ProfilePageLink.Should().Be(profilePageLink);
            }
        }
    }
}
