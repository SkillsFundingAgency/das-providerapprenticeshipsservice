using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenSubmittingReviewApprenticeshipUpdate
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<ReviewApprenticeshipUpdateCommand>()))
                .ReturnsAsync(() => new Unit());

            _orchestrator = new ManageApprenticesOrchestrator(
                _mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                Mock.Of<IApprenticeshipMapper>(),
                Mock.Of<IApprovedApprenticeshipValidator>()
                );
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ShouldCallMediatorToSubmitReview(bool isApproved)
        {
            //Arrange
            var providerId = 1;
            var apprenticeshipId = "appid";
            var userId = "tester";

            //Act
            await _orchestrator.SubmitReviewApprenticeshipUpdate(providerId, apprenticeshipId, userId, isApproved);

            //Assert
            _mediator.Verify(x => x.SendAsync(
                It.Is<ReviewApprenticeshipUpdateCommand>(r =>
                    r.IsApproved == isApproved
                    && r.ProviderId == providerId
                    && r.UserId == userId
                )), Times.Once());
        }
    }
}
