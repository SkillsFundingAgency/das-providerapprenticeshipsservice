using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UndoApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenSubmittingUndoApprenticeshipUpdate
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<UndoApprenticeshipUpdateCommand>()))
                .ReturnsAsync(() => new Unit());

            _orchestrator = new ManageApprenticesOrchestrator(
                _mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                Mock.Of<IApprenticeshipMapper>(),
                Mock.Of<IApprovedApprenticeshipValidator>(),
                Mock.Of<IApprenticeshipFiltersMapper>(),
                Mock.Of<IDataLockMapper>(),
                Mock.Of<IFiltersCookieManager>(),
                null);
        }


        [TestCase(true)]
        [TestCase(false)]
        public async Task ShouldCallMediatorToSubmitUndo(bool isApproved)
        {
            //Arrange
            var providerId = 1;
            var apprenticeshipId = "appid";
            var userId = "tester";
            var loginUser = new SignInUserModel { DisplayName = "Bob", Email = "test@email.com" };

            //Act
            await _orchestrator.SubmitUndoApprenticeshipUpdate(providerId, apprenticeshipId, userId, loginUser);

            //Assert
            _mediator.Verify(x => x.SendAsync(
                It.Is<UndoApprenticeshipUpdateCommand>(r =>
                    r.ProviderId == providerId
                    && r.UserId == userId
                    && r.UserDisplayName == loginUser.DisplayName
                    && r.UserEmailAddress == loginUser.Email
                )), Times.Once());
        }

    }
}
