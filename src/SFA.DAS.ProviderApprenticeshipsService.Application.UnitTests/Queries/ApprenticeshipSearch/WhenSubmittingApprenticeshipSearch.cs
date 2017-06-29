using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.ApprenticeshipSearch
{
    [TestFixture()]
    public class WhenSubmittingApprenticeshipSearch
    {
        private ApprenticeshipSearchQueryHandler _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(
                x => x.GetProviderApprenticeships(It.IsAny<long>(), It.IsAny<ApprenticeshipSearchQuery>()))
                .ReturnsAsync(new ApprenticeshipSearchResponse
                {
                    Apprenticeships = new List<Apprenticeship>()
                });

            _handler = new ApprenticeshipSearchQueryHandler(_commitmentsApi.Object,
                Mock.Of<IProviderCommitmentsLogger>());
        }

        [Test]
        public async Task ThenCommitmentsApiIsCalled()
        {
            //Arrange
            var request = new ApprenticeshipSearchQueryRequest
            {
                ProviderId = 1,
                Query = new ApprenticeshipSearchQuery()
            };

            //Act
            await _handler.Handle(request);

            //Assert
            _commitmentsApi.Verify(
                x => x.GetProviderApprenticeships(
                    It.IsAny<long>(),
                    It.IsAny<ApprenticeshipSearchQuery>()),
                    Times.Once);
        }

        [Test]
        public async Task ThenCommitmentsApiIsCalledWithCorrectPageNumber()
        {
            //Arrange
            var request = new ApprenticeshipSearchQueryRequest
            {
                ProviderId = 1,
                Query = new ApprenticeshipSearchQuery { PageNumber = 5 }
            };

            //Act
            await _handler.Handle(request);

            //Assert
            _commitmentsApi.Verify(
                x => x.GetProviderApprenticeships(
                    It.IsAny<long>(),
                    It.Is<ApprenticeshipSearchQuery>(a => a.PageNumber == 5)),
                    Times.Once);
            }
        }
    }
