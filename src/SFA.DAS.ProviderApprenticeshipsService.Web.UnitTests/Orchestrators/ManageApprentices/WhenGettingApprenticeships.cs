using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenGettingApprenticeships
    {
        [Test, MoqCustomisedAutoData]
        public async Task ThenUsesFilterCookieManager(
            long providerId,
            ApprenticeshipFiltersViewModel filtersViewModel,
            string externalUserId,
            ApprenticeshipSearchQueryResponse apprenticeshipSearchQueryResponse,
            [Frozen] Mock<IFiltersCookieManager> mockFilterCookieManager,
            [Frozen] Mock<IMediator> mockMediator,
            ManageApprenticesOrchestrator sut)
        {
            mockMediator
                .Setup(mediator => mediator.Send(It.IsAny<ApprenticeshipSearchQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeshipSearchQueryResponse);
            await sut.GetApprenticeships(providerId, filtersViewModel);
            mockFilterCookieManager.Verify(manager => manager.SetCookie(filtersViewModel));
        }
    }
}