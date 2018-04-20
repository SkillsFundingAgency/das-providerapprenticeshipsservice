using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithProvider
    {
        [TestCase(RequestStatus.ReadyForReview, LastAction.None, AgreementStatus.NotAgreed, TestName = "Review saved by provider without send")]
        [TestCase(RequestStatus.ReadyForReview, LastAction.Amend, AgreementStatus.NotAgreed, TestName = "Request sent for review with no agreemeent from either party")]
        [TestCase(RequestStatus.ReadyForApproval, LastAction.Approve, AgreementStatus.EmployerAgreed, TestName = "Ready for approval")]
        [TestCase(RequestStatus.ReadyForReview, LastAction.Approve, AgreementStatus.NotAgreed, TestName = "Sent for approval but a change has been made")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus agreementStatus)
        {
            var commitment = new CommitmentListItem
            {
                LastAction = lastAction,
                AgreementStatus = agreementStatus,

                EditStatus = EditStatus.ProviderOnly,
                ApprenticeshipCount = 2,
                ProviderLastUpdateInfo = new LastUpdateInfo
                {
                    EmailAddress = "test@testcorp",
                    Name = "Test"
                }
            };

            var status = commitment.GetStatus();

            status.Should().Be(expectedResult);
        }

        [TestCase(LastAction.None, AgreementStatus.NotAgreed)]
        [TestCase(LastAction.Amend, AgreementStatus.NotAgreed)]
        [TestCase(LastAction.Approve, AgreementStatus.EmployerAgreed)]
        public void WhenTheProviderHasNeverModifiedTheCommitmentItIsClassedAsNew(LastAction lastAction, AgreementStatus agreementStatus)
        {
            var commitment = new CommitmentListItem
            {
                LastAction = lastAction,
                AgreementStatus = agreementStatus,

                EditStatus = EditStatus.ProviderOnly,
                ApprenticeshipCount = 2
            };

            //Act
            var status = commitment.GetStatus();

            //Assert
            Assert.AreEqual(RequestStatus.NewRequest, status);
        }
    }
}
