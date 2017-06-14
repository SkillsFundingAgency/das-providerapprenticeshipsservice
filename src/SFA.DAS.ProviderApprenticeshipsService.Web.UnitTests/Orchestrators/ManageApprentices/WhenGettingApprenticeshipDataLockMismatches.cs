using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenGettingApprenticeshipDataLockMismatches
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IApprenticeshipMapper> _mapper;

        [SetUp]
        public void Arrange()
        {
           _mediator = new Mock<IMediator>();
           _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipPriceHistoryQueryRequest>()))
                .ReturnsAsync(() => new GetApprenticeshipPriceHistoryQueryResponse
                {
                    History = new List<PriceHistory>()
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipDataLocksRequest>()))
                .ReturnsAsync(() => new GetApprenticeshipDataLocksResponse
                {
                    Data = new List<DataLockStatus>()
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(() => new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipDataLockSummaryQueryRequest>()))
                .ReturnsAsync(() => new GetApprenticeshipDataLockSummaryQueryResponse
                {
                    DataLockSummary = new DataLockSummary()
                });

            _mapper = new Mock<IApprenticeshipMapper>();
            _mapper.Setup(x => x.MapDataLockSummary(It.IsAny<DataLockSummary>()))
                .ReturnsAsync(() => new DataLockSummaryViewModel
                {
                    DataLockWithCourseMismatch = new List<DataLockViewModel>(),
                    DataLockWithOnlyPriceMismatch = new List<DataLockViewModel>()
                });

            _orchestrator = new ManageApprenticesOrchestrator(
                _mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _mapper.Object,
                Mock.Of<IApprovedApprenticeshipValidator>(),
                Mock.Of<IApprenticeshipFiltersMapper>()
                );
        }

        [Test]
        public async Task ThenMediatorIsCalledToRetrievePriceHistory()
        {
            //Act
            await _orchestrator.GetApprenticeshipMismatchDataLock(1, "TEST");

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetApprenticeshipPriceHistoryQueryRequest>()),
                Times.Once);
        }

        [Test]
        public async Task ThenMediatorIsCalledToRetrieveDataLockSummary()
        {
            //Act
            await _orchestrator.GetApprenticeshipMismatchDataLock(1, "TEST");

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetApprenticeshipDataLockSummaryQueryRequest>()), Times.Once());
        }

        [Test]
        public async Task ThenMapperIsCalledToMapDataLockSummaryToViewModel()
        {
            //Act
            await _orchestrator.GetApprenticeshipMismatchDataLock(1, "TEST");

            //Assert
            _mapper.Verify(x => x.MapDataLockSummary(It.IsAny<DataLockSummary>()), Times.Once);
        }
    }
}
