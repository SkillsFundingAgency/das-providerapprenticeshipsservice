using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.DataLock
{
    [TestFixture]
    public class WhenGettingApprenticeshipDataLockMismatches
    {
        private DataLockOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IDataLockMapper> _mapper;

        private const long EventId = 123L;

        [SetUp]
        public void Arrange()
        {
           _mediator = new Mock<IMediator>();
           _mediator.Setup(x => x.Send(It.IsAny<GetApprenticeshipPriceHistoryQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetApprenticeshipPriceHistoryQueryResponse
                {
                    History = new List<PriceHistory>()
                });

            _mediator.Setup(x => x.Send(It.IsAny<GetApprenticeshipQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            _mediator.Setup(x => x.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView()
                });

            _mediator.Setup(x => x.Send(It.IsAny<GetApprenticeshipDataLockSummaryQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetApprenticeshipDataLockSummaryQueryResponse
                {
                    DataLockSummary = new DataLockSummary()
                });

            _mapper = new Mock<IDataLockMapper>();
           
            // Arrange
            _mapper.Setup(x => x.MapDataLockSummary(It.IsAny<DataLockSummary>(), It.IsAny<bool>()))
                .ReturnsAsync(() => new DataLockSummaryViewModel
                {
                    DataLockWithCourseMismatch = new List<DataLockViewModel>
                    {
                        new DataLockViewModel {DataLockEventId = EventId, DataLockErrorCode = DataLockErrorCode.Dlock07, IlrEffectiveFromDate = DateTime.Today, TriageStatusViewModel = TriageStatusViewModel.Unknown}
                    },
                    DataLockWithOnlyPriceMismatch = new List<DataLockViewModel>()
                });

            _orchestrator = new DataLockOrchestrator(
                _mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                Mock.Of<IApprenticeshipMapper>(),
                _mapper.Object);
        }

        [Test]
        public async Task ThenMediatorIsCalledToRetrievePriceHistory()
        {
            //Act
            await _orchestrator.GetApprenticeshipMismatchDataLock(1, "TEST");

            //Assert
            _mediator.Verify(x => x.Send(It.IsAny<GetApprenticeshipPriceHistoryQueryRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task ThenMediatorIsCalledToRetrieveDataLockSummary()
        {
            //Act
            await _orchestrator.GetApprenticeshipMismatchDataLock(1, "TEST");

            //Assert
            _mediator.Verify(x => x.Send(It.IsAny<GetApprenticeshipDataLockSummaryQueryRequest>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task ThenMapperIsCalledToMapDataLockSummaryToViewModel()
        {
            //Act
            await _orchestrator.GetApprenticeshipMismatchDataLock(1, "TEST");

            //Assert
            _mapper.Verify(x => x.MapDataLockSummary(It.IsAny<DataLockSummary>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task ThenConfirmRestartGetsCorrectDataLockEvent()
        {
            // Act
            var result = await _orchestrator.GetConfirmRestartViewModel(1, "TEST");

            // Assert
            Assert.AreEqual(EventId, result.DataLockEventId);
        }
    }
}
