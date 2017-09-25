using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using MediatR;

using Moq;

using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGetCohorts : ApprenticeshipValidationTestBase
    {
       public Task<GetCommitmentsQueryResponse> _commitments;


        protected override void SetUp()
        {
            _commitments = Task.FromResult(TestData());
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>())).Returns(_commitments);
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetProviderAgreementQueryRequest>()))
                .Returns(Task.FromResult(new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed }));
        }

        [Test]
        public async Task TestHappyPath()
        {
            await _orchestrator.GetCohorts(1234567);

            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()), Times.AtLeastOnce);
            _mockCalculator.Verify(m => m.GetStatus(It.IsAny<EditStatus>(), It.IsAny<int>(), It.IsAny<LastAction>(), It.IsAny<AgreementStatus>(), It.IsAny<LastUpdateInfo>()), Times.Exactly(_commitments.Result.Commitments.Count));
        }
        

        [Test]
        public async Task TestFilter()
        {
            SetUpOrchestrator(new CommitmentStatusCalculator());

            var result = await _orchestrator.GetCohorts(1234567);

            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()), Times.Once);

            result.WithEmployerCount.Should().Be(2);
            result.ReadyForReviewCount.Should().Be(6);
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
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.None,
                                                      ProviderLastUpdateInfo = new LastUpdateInfo()
                                                    },
                                                 new CommitmentListItem // ReadyForApproval
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Approve,
                                                      ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "a@b", Name="Test"}
                                                    },
                                                 new CommitmentListItem // ReadyForApproval
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Approve,
                                                      ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "a@b", Name="Test"}
                                                    },
                                                 new CommitmentListItem // With employer
                                                    { AgreementStatus = AgreementStatus.NotAgreed, ApprenticeshipCount = 6,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.EmployerOnly, LastAction = LastAction.Amend,
                                                      ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "a@b", Name="Test"}
                                                    },
                                                 new CommitmentListItem // With employer
                                                    { AgreementStatus = AgreementStatus.ProviderAgreed, ApprenticeshipCount = 6,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.EmployerOnly, LastAction = LastAction.Approve,
                                                      ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "a@b", Name="Test"}
                                                    },
                                                 new CommitmentListItem // ReadyForReview
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = false, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Amend,
                                                      ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "a@b", Name="Test"}
                                                    },
                                                 new CommitmentListItem // ReadyForReview
                                                    { AgreementStatus = AgreementStatus.EmployerAgreed, ApprenticeshipCount = 5,
                                                      CanBeApproved = false, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.Amend,
                                                      ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "a@b", Name="Test"}
                                                    },
                                                 new CommitmentListItem // ReadyForReview
                                                    { AgreementStatus = AgreementStatus.NotAgreed, ApprenticeshipCount = 0,
                                                      CanBeApproved = true, CommitmentStatus = CommitmentStatus.Active,
                                                      EditStatus = EditStatus.ProviderOnly, LastAction = LastAction.None,
                                                      ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "a@b", Name="Test"}
                                                    }
                                             }
                       };
        }
    }
}
