using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetPendingApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGettingReviewApprenticeshipUpdate
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IApprenticeshipMapper> _apprenticeshipMapper;

        private GetPendingApprenticeshipUpdateQueryResponse _pendingApprenticeshipUpdate;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<ReviewApprenticeshipUpdateCommand>()))
                .ReturnsAsync(() => new Unit());

            _pendingApprenticeshipUpdate = new GetPendingApprenticeshipUpdateQueryResponse
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate
                {
                    ApprenticeshipId = 1,
                    Originator = Originator.Employer,
                    LastName = "Updated"
                }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPendingApprenticeshipUpdateQueryRequest>()))
                .ReturnsAsync(() => _pendingApprenticeshipUpdate);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(() => new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            _apprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            _apprenticeshipMapper.Setup(x =>
                        x.MapApprenticeshipUpdateViewModel<ReviewApprenticeshipUpdateViewModel>(
                            It.IsAny<Apprenticeship>(), It.IsAny<ApprenticeshipUpdate>()))
                .Returns(new ReviewApprenticeshipUpdateViewModel());

            _orchestrator = new ManageApprenticesOrchestrator(
                _mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _apprenticeshipMapper.Object,
                Mock.Of<IApprovedApprenticeshipValidator>()
                );
        }

        [Test]
        public void ShouldAssertProviderCannotReviewOwnUpdate()
        {
            //Arrange
            _pendingApprenticeshipUpdate.ApprenticeshipUpdate.Originator = Originator.Provider;

            //Act
            Func<Task> act = async () => await _orchestrator.GetReviewApprenticeshipUpdateModel(0, "");
            act.ShouldNotThrow<ValidationException>();
        }

        [Test]
        public async Task ShouldCallMediatorToGetTheOriginalApprenticeship()
        {
            //Act
            await _orchestrator.GetReviewApprenticeshipUpdateModel(0, "");

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);
        }

        [Test]
        public async Task ShouldCallMapperToGenerateTheViewModel()
        {
            //Act
            await _orchestrator.GetReviewApprenticeshipUpdateModel(0, "");

            //Assert
            _apprenticeshipMapper.Verify(x =>
                x.MapApprenticeshipUpdateViewModel<ReviewApprenticeshipUpdateViewModel>(
                    It.IsAny<Apprenticeship>(), It.IsAny<ApprenticeshipUpdate>()), Times.Once);
        }
    }
}
