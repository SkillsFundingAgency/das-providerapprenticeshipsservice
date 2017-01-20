using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using MediatR;

using Moq;

using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class WhenGetCohorts
    {
        [Test]
        public async Task TestFilter()
        {
            var mockMediator = new Mock<IMediator>();
            var commitments = Task.FromResult(TestData());
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>())).Returns(commitments);
            // ICommitmentStatusCalculator
            var mockCalculator = new Mock<ICommitmentStatusCalculator>();

            var sut = new CommitmentOrchestrator(mockMediator.Object, new CommitmentStatusCalculator(), Mock.Of<IHashingService>(), Mock.Of<IProviderCommitmentsLogger>());

            var result = await sut.GetCohorts(1234567);

            result.NewRequestsCount.Should().Be(0);

        }

        private static GetCommitmentsQueryResponse TestData()
        {
            return new GetCommitmentsQueryResponse
                       {
                           Commitments = new List<CommitmentListItem>
                                             {
                                                 new CommitmentListItem
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Approve
                                                    }
                                             }
                       };
        }
    }
}
