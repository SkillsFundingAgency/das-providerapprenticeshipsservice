using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Authentication
{
    [TestFixture]
    public class WhenSavingIdentityAttributes
    {
        private AuthenticationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<UpsertRegisteredUserCommand>()))
                .ReturnsAsync(new Unit());

            _orchestrator = new AuthenticationOrchestrator(_mediator.Object, Mock.Of<IProviderCommitmentsLogger>());
        }

        [Test]
        public async Task TheMediatorIsCalled()
        {
            //Act
            await _orchestrator.SaveIdentityAttributes("UserId", "UkPrn", "DisplayName", "Email");

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpsertRegisteredUserCommand>()));
        }
    }
}
