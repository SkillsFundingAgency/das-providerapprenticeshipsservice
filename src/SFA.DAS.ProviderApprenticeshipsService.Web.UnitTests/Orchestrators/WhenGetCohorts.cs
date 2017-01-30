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
        public async Task TestHappyPath()
        {
            var mockMediator = new Mock<IMediator>();
            var mockCalculator = new Mock<ICommitmentStatusCalculator>();
            var commitments = Task.FromResult(TestData());
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>())).Returns(commitments);

            var sut = new CommitmentOrchestrator(mockMediator.Object, mockCalculator.Object, Mock.Of<IHashingService>(), Mock.Of<IProviderCommitmentsLogger>());

            await sut.GetCohorts(1234567);

            mockMediator.Verify(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()), Times.Once);
            mockCalculator.Verify(m => m.GetStatus(It.IsAny<EditStatus>(), It.IsAny<int>(), It.IsAny<LastAction>(), It.IsAny<AgreementStatus>()), Times.Exactly(commitments.Result.Commitments.Count));
        }

        [Test]
        public async Task TestFilter()
        {
            var mockMediator = new Mock<IMediator>();
            var commitments = Task.FromResult(TestData());
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>())).Returns(commitments);

            var sut = new CommitmentOrchestrator(mockMediator.Object, new CommitmentStatusCalculator(), Mock.Of<IHashingService>(), Mock.Of<IProviderCommitmentsLogger>());

            var result = await sut.GetCohorts(1234567);

            mockMediator.Verify(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()), Times.Once);

            result.NewRequestsCount.Should().Be(1);
            result.ReadyForApprovalCount.Should().Be(2);
            result.WithEmployerCount.Should().Be(2);
            result.ReadyForReviewCount.Should().Be(2);
        }

        private static GetCommitmentsQueryResponse TestData()
        {
            return new GetCommitmentsQueryResponse
                       {
                           Commitments = new List<CommitmentListItem>
                                             {
                                                new CommitmentListItem // NewRequest
                                                    { AgreementStatus = AgreementStatus.NotAgreed, ApprenticeshipCount = 0,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Amend
                                                    },
                                                 new CommitmentListItem // ReadyForApproval
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Approve
                                                    },
                                                 new CommitmentListItem // ReadyForApproval
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Approve
                                                    },
                                                 new CommitmentListItem // With employer
                                                    { AgreementStatus = AgreementStatus.NotAgreed, ApprenticeshipCount = 6,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.EmployerOnly, LastAction = LastAction.Amend
                                                    },
                                                 new CommitmentListItem // With employer
                                                    { AgreementStatus = AgreementStatus.ProviderAgreed, ApprenticeshipCount = 6,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.EmployerOnly, LastAction = LastAction.Approve
                                                    },
                                                 new CommitmentListItem // ReadyForReview
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = false, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Amend
                                                    },
                                                 new CommitmentListItem // ReadyForReview
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = false, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Amend
                                                    }
                                             }
                       };
        }
    }
}
