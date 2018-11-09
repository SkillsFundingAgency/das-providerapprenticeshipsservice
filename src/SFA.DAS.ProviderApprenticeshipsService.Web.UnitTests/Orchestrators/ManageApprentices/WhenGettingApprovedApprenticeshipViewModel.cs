using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprovedApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenGettingApprovedApprenticeshipViewModel
    {
        private Mock<IHashingService> _mockHashingService;
        private Mock<IMediator> _mockMediator;
        private Mock<IApprovedApprenticeshipMapper> _mockMapper;
        private Mock<IFiltersCookieManager> _mockFiltersCookieManager;
        private Mock<IDataLockMapper> _mockDataLockMapper;
        private Mock<IProviderCommitmentsLogger> _mockLogger;

        private long _expectedProviderId;
        private string _expectedHashedApprenticeId;
        private long _expectedApprenticeId;
        private string _expectedLastName;
        private long _expectedDataLockEventId;
        private string _expectedSearchInput;
        private bool _expectedDataLockSuccess;

        private ApprovedApprenticeshipViewModel _result;
        
        [SetUp]
        public async Task GivenAManageApprenticesOrchestrator_WhenGettingApprovedApprenticeshipViewModel()
        {
            _expectedProviderId = 112;
            _expectedHashedApprenticeId = "testhash114";
            _expectedApprenticeId = 114;
            _expectedLastName = "Pelina";
            _expectedDataLockEventId = 116;
            _expectedSearchInput = "testsearch";
            _expectedDataLockSuccess = true;

            _mockHashingService = new Mock<IHashingService>();
            _mockMediator = new Mock<IMediator>();
            _mockMapper = new Mock<IApprovedApprenticeshipMapper>();
            _mockFiltersCookieManager = new Mock<IFiltersCookieManager>();
            _mockDataLockMapper = new Mock<IDataLockMapper>();
            _mockLogger = new Mock<IProviderCommitmentsLogger>();
            

            _mockHashingService.Setup(x => x.DecodeValue(_expectedHashedApprenticeId)).Returns(_expectedApprenticeId);
            _mockMediator
                .Setup(x => x.SendAsync(It.Is<GetApprovedApprenticeshipQueryRequest>(
                    y => y.ApprovedApprenticeshipId == _expectedApprenticeId && y.ProviderId == _expectedProviderId)))
                .ReturnsAsync(new GetApprovedApprenticeshipQueryResponse()
                {
                    ApprovedApprenticeship = new ApprovedApprenticeship {LastName = _expectedLastName, HasHadDataLockSuccess = _expectedDataLockSuccess }
                });
            _mockMediator
                .Setup(x => x.SendAsync(It.Is<GetApprenticeshipDataLockSummaryQueryRequest>(
                    y => y.ProviderId == _expectedProviderId && y.ApprenticeshipId == _expectedApprenticeId)))
                .ReturnsAsync(new GetApprenticeshipDataLockSummaryQueryResponse()
                {
                    DataLockSummary = new DataLockSummary
                    {
                        DataLockWithCourseMismatch = new List<DataLockStatus>()
                        {
                            new DataLockStatus {DataLockEventId = _expectedDataLockEventId}
                        }
                    }
                });
            _mockMapper
                .Setup(x => x.Map(It.Is<ApprovedApprenticeship>(y => y.LastName == _expectedLastName)))
                .Returns(new ApprovedApprenticeshipViewModel {LastName = _expectedLastName, HasHadDataLockSuccess = _expectedDataLockSuccess });
            _mockFiltersCookieManager.Setup(x => x.GetCookie())
                .Returns(new ApprenticeshipFiltersViewModel {SearchInput = _expectedSearchInput});
            _mockDataLockMapper
                .Setup(x => x.MapDataLockSummary(It.Is<DataLockSummary>(
                        y => y.DataLockWithCourseMismatch.Any(z => z.DataLockEventId == _expectedDataLockEventId)),
                    _expectedDataLockSuccess))
                .ReturnsAsync(new DataLockSummaryViewModel()
                {
                    DataLockWithCourseMismatch = new List<DataLockViewModel>()
                    {
                        new DataLockViewModel {DataLockEventId = _expectedDataLockEventId}
                    }
                });

            var orcestrator = new ManageApprenticesOrchestrator(
                _mockMediator.Object,
                _mockHashingService.Object,
                _mockLogger.Object,
                null,
                null,
                null,
                _mockDataLockMapper.Object,
                _mockFiltersCookieManager.Object,
                _mockMapper.Object);

            _result = await orcestrator.GetApprovedApprenticeshipViewModel(_expectedProviderId, _expectedHashedApprenticeId);
        }

        [Test]
        public void ThenTheHashingServiceIsCalledCorrectly()
        {
            _mockHashingService.Verify(x => x.DecodeValue(_expectedHashedApprenticeId));
        }

        [Test]
        public void ThenTheMediatorIsCalledCorrectlyForGetApprovedApprenticeshipQueryRequest()
        {
            _mockMediator
                .Verify(x => x.SendAsync(It.Is<GetApprovedApprenticeshipQueryRequest>(
                    y => y.ApprovedApprenticeshipId == _expectedApprenticeId && y.ProviderId == _expectedProviderId)));
        }

        [Test]
        public void ThenTheMediatorIsCalledCorrectlyForGetApprenticeshipDataLockSummaryQueryRequest()
        {
            _mockMediator
                .Verify(x => x.SendAsync(It.Is<GetApprenticeshipDataLockSummaryQueryRequest>(
                    y => y.ProviderId == _expectedProviderId && y.ApprenticeshipId == _expectedApprenticeId)));
        }

        [Test]
        public void ThenTheApprovedApprenticeshipMapperIsCalledCorrectly()
        {
            _mockMapper
                .Setup(x => x.Map(It.Is<ApprovedApprenticeship>(y => y.LastName == _expectedLastName)));
        }

        [Test]
        public void ThenTheFiltersCookieManagerIsCalledCorrectly()
        {
            _mockFiltersCookieManager.Verify(x => x.GetCookie());
        }

        [Test]
        public void ThenTheDataLockMapperIsCalledCorrectly()
        {
            _mockDataLockMapper
                .Verify(x => x.MapDataLockSummary(It.Is<DataLockSummary>(
                        y => y.DataLockWithCourseMismatch.Any(z => z.DataLockEventId == _expectedDataLockEventId)),
                    _expectedDataLockSuccess));
        }

        [Test]
        public void ThenTheResultHasTheExpectedApprenticeshipReturned()
        {
            Assert.That(_result.LastName, Is.EqualTo(_expectedLastName));
        }

        [Test]
        public void ThenTheResultHasTheExpectedSearchFiltersReturned()
        {
            Assert.That(_result.SearchFiltersForListView.SearchInput, Is.EqualTo(_expectedSearchInput));
        }

        [Test]
        public void ThenTheResultHasTheExpectedDataLockSummaryReturned()
        {
            Assert.That(_result.DataLockSummaryViewModel.DataLockWithCourseMismatch.Any(x => x.DataLockEventId == _expectedDataLockEventId));
        }
    }
}
