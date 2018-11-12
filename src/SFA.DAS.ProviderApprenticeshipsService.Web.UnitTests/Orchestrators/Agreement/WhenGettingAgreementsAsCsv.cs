using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Formatters;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Agreement
{
    [TestFixture]
    public class WhenGettingAgreementsAsCsv
    {
        [Test, MoqCustomisedAutoData]
        public async Task ThenReturnsByteArrayFromCsvFormatter(
            long providerId,
            byte[] expectedResult,
            GetCommitmentAgreementsQueryResponse getAgreementsResponse,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<ICsvFormatter> mockCsvFormatter,
            AgreementOrchestrator sut)
        {
            mockMediator
                .Setup(mediator =>
                    mediator.Send(It.Is<GetCommitmentAgreementsQueryRequest>(request =>
                        request.ProviderId == providerId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(getAgreementsResponse);

            mockCsvFormatter
                .Setup(formatter => formatter.Format(It.IsAny<IEnumerable<CommitmentAgreement>>()))
                .Returns(expectedResult);

            var result = await sut.GetAgreementsAsCsv(providerId);

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}