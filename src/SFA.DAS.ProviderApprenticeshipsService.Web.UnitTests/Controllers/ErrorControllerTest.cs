using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Error;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ErrorControllerTest
    {
        private ErrorController _controller;
        private Mock<IConfiguration> _mockConfiguration;
        private ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;
        

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _providerApprenticeshipsServiceConfiguration = new ProviderApprenticeshipsServiceConfiguration();
            _controller = new ErrorController(_providerApprenticeshipsServiceConfiguration, _mockConfiguration.Object);
        }

        [Test]
        [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
        [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
        [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        public void Forbidden_Shows_Correct_View_When_UseDfESignIn_True(string env, string helpLink, bool useDfESignIn)
        {
            //arrange
            _mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
            _providerApprenticeshipsServiceConfiguration.UseDfESignIn = useDfESignIn;

            //sut
            var actual = _controller.Forbidden();

            Assert.That(actual, Is.Not.Null);
            var actualModel = actual?.Model as Error403ViewModel;
            Assert.AreEqual(helpLink, actualModel?.HelpPageLink);
            Assert.AreEqual(useDfESignIn, actualModel?.UseDfESignIn);
        }
    }
}
