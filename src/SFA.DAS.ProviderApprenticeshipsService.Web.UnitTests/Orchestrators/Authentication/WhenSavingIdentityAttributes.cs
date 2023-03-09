using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.UserIdentityService;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Authentication
{
    [TestFixture]
    public class WhenSavingIdentityAttributes
    {
        private AuthenticationOrchestrator _orchestrator;
        private Mock<IUserIdentityService> _userIdentityService;
        private Mock<IProviderCommitmentsLogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _userIdentityService = new Mock<IUserIdentityService>();
            _logger = new Mock<IProviderCommitmentsLogger>();
            _userIdentityService.Setup(x => x.UpsertUserIdentityAttributes(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(Unit.Value));

            _orchestrator = new AuthenticationOrchestrator(_logger.Object, _userIdentityService.Object);
        }

        [Test]
        public async Task SaveIdentityAttributesIsCalledAndReturnsTrue()
        {
            // Arrange
            var ukprn = "12345";

            //Act
            var result = await _orchestrator.SaveIdentityAttributes("UserRef", ukprn, "DisplayName", "Email");

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SaveIdentityAttributesIsCalledAndReturnsFalse()
        {
            // Arrange
            var ukprn = "12345x";

            //Act
            var result = await _orchestrator.SaveIdentityAttributes("UserRef", ukprn, "DisplayName", "Email");

            //Assert
            Assert.IsFalse(result);
        }
    }
}
