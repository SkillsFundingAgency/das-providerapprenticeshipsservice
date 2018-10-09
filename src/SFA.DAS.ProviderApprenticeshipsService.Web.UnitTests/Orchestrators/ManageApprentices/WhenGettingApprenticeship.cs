using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenGettingApprenticeship
    {
        [Test, MoqCustomisedAutoData]
        public async Task ThenSetsFilterFromCookie(
            long providerId,
            string hashedAccountId,
            string hashedApprenticeshipId,
            ApprenticeshipFiltersViewModel filtersViewModel,
            string externalUserId,
            GetApprenticeshipQueryResponse getApprenticeshipQueryResponse,
            GetApprenticeshipDataLockSummaryQueryResponse apprenticeshipDataLockSummaryQueryResponse,
            ApprenticeshipDetailsViewModel apprenticeshipDetailsViewModel,
            [Frozen] Mock<IFiltersCookieManager> mockFilterCookieManager,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IApprenticeshipMapper> mockMapper,
            ManageApprenticesOrchestrator sut)
        {
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(getApprenticeshipQueryResponse);
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetApprenticeshipDataLockSummaryQueryRequest>()))
                .ReturnsAsync(apprenticeshipDataLockSummaryQueryResponse);

            mockMapper
                .Setup(mapper => mapper.MapApprenticeshipDetails(It.IsAny<Apprenticeship>()))
                .Returns(apprenticeshipDetailsViewModel);

            mockFilterCookieManager
                .Setup(manager => manager.GetCookie())
                .Returns(filtersViewModel);

            var result = await sut.GetApprenticeshipViewModel(providerId, hashedApprenticeshipId);

            result.SearchFiltersForListView.Should().BeSameAs(filtersViewModel);
        }
    }
}