using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenGettingApprenticeships
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mockMediator;
        private Mock<IApprenticeshipMapper> _mockApprenticeshipMapper;
        private Mock<IApprenticeshipFiltersMapper> _mockApprenticeshipFiltersMapper;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<ApprenticeshipSearchQueryRequest>()))
                .ReturnsAsync(() => new ApprenticeshipSearchQueryResponse
                {
                    Apprenticeships =  new List<Apprenticeship>(),
                    Facets =  new Facets()
                });

            _mockApprenticeshipMapper = new Mock<IApprenticeshipMapper>();

            _mockApprenticeshipFiltersMapper = new Mock<IApprenticeshipFiltersMapper>();

            _mockApprenticeshipFiltersMapper.Setup(
                x => x.MapToApprenticeshipSearchQuery(It.IsAny<ApprenticeshipFiltersViewModel>()))
                .Returns(new ApprenticeshipSearchQuery { PageNumber = 5 });

            _mockApprenticeshipFiltersMapper.Setup(
                x => x.Map(It.IsAny<Facets>()))
                .Returns(new ApprenticeshipFiltersViewModel());


            _orchestrator = new ManageApprenticesOrchestrator(
                _mockMediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _mockApprenticeshipMapper.Object,
                Mock.Of<IApprovedApprenticeshipValidator>(),
                _mockApprenticeshipFiltersMapper.Object,
                Mock.Of<IDataLockMapper>());
        }

        [Test]
        public async Task ThenShouldMapFiltersToSearchQuery()
        {
            //Act
            await _orchestrator.GetApprenticeships(1, new ApprenticeshipFiltersViewModel());

            //Assert
            _mockApprenticeshipFiltersMapper.Verify(
                x => x.MapToApprenticeshipSearchQuery(It.IsAny<ApprenticeshipFiltersViewModel>())
                , Times.Once());
        }

        [Test]
        public async Task ThenShouldMapSearchResultsToViewModel()
        {
            //Act
            await _orchestrator.GetApprenticeships(1, new ApprenticeshipFiltersViewModel());

            //Assert
            _mockApprenticeshipFiltersMapper.Verify(
                x => x.Map(It.IsAny<Facets>())
                , Times.Once());
        }


        [Test]
        public async Task ThenShouldCallMediatorToQueryApprenticeships()
        {
            //Act
            await _orchestrator.GetApprenticeships(1, new ApprenticeshipFiltersViewModel());

            //Assert
            _mockMediator.Verify(x => x.SendAsync(It.IsAny<ApprenticeshipSearchQueryRequest>()), Times.Once);
        }
    }
}
