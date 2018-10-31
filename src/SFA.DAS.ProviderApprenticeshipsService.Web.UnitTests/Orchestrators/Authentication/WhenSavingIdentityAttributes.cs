using System.Threading;
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
            _mediator.Setup(x => x.Send(It.IsAny<UpsertRegisteredUserCommand>(), new CancellationToken()))
                .ReturnsAsync(new Unit());

            _orchestrator = new AuthenticationOrchestrator(_mediator.Object, Mock.Of<IProviderCommitmentsLogger>());
        }

        [Test]
        public async Task TheMediatorIsCalled()
        {
            //Act
            await _orchestrator.SaveIdentityAttributes("UserRef", 12345, "DisplayName", "Email");

            //Assert
            _mediator.Verify(x => x.Send(It.IsAny<UpsertRegisteredUserCommand>(), It.IsAny<CancellationToken>()));
        }
    }
}
