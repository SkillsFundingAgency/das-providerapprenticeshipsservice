﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UndoApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetPendingApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenGettingUndoApprenticeshipUpdate
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IApprenticeshipMapper> _apprenticeshipMapper;
        private GetPendingApprenticeshipUpdateQueryResponse _pendingApprenticeshipUpdate;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<UndoApprenticeshipUpdateCommand>(), new CancellationToken()))
                .ReturnsAsync(() => new Unit());

            _pendingApprenticeshipUpdate = new GetPendingApprenticeshipUpdateQueryResponse
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate
                {
                    ApprenticeshipId = 1,
                    Originator = Originator.Provider,
                    LastName = "Updated"
                }
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetPendingApprenticeshipUpdateQueryRequest>(), new CancellationToken()))
                .ReturnsAsync(() => _pendingApprenticeshipUpdate);

            _mediator.Setup(x => x.Send(It.IsAny<GetApprenticeshipQueryRequest>(), new CancellationToken()))
                .ReturnsAsync(() => new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            _apprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            _apprenticeshipMapper.Setup(x =>
                        x.MapApprenticeshipUpdateViewModel<UndoApprenticeshipUpdateViewModel>(
                            It.IsAny<Apprenticeship>(), It.IsAny<ApprenticeshipUpdate>()))
                .Returns(new UndoApprenticeshipUpdateViewModel());

            _orchestrator = new ManageApprenticesOrchestrator(
                _mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _apprenticeshipMapper.Object,
                Mock.Of<IApprovedApprenticeshipValidator>(),
                Mock.Of<IDataLockMapper>());
        }

        [Test]
        public void ShouldAssertProviderCannotUndoEmployerUpdate()
        {
            //Arrange
            _pendingApprenticeshipUpdate.ApprenticeshipUpdate.Originator = Originator.Employer;

            //Act
            Func<Task> act = async () => await _orchestrator.GetUndoApprenticeshipUpdateModel(0, "");
            act.Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldCallMediatorToGetTheOriginalApprenticeship()
        {
            //Act
            await _orchestrator.GetUndoApprenticeshipUpdateModel(0, "");

            //Assert
            _mediator.Verify(x => x.Send(It.IsAny<GetApprenticeshipQueryRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ShouldCallMapperToGenerateTheViewModel()
        {
            //Act
            await _orchestrator.GetUndoApprenticeshipUpdateModel(0, "");

            //Assert
            _apprenticeshipMapper.Verify(x =>
                x.MapApprenticeshipUpdateViewModel<UndoApprenticeshipUpdateViewModel>(
                    It.IsAny<Apprenticeship>(), It.IsAny<ApprenticeshipUpdate>()), Times.Once);
        }
    }
}
