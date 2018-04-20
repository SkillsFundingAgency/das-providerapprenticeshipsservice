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
    public sealed class WhenEditStatusIsWithEmployer
    {
        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.NotAgreed, TestName = "Sent request for review to employer")]
        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.EmployerAgreed, TestName = "Sent back approved request for review")]
        [TestCase(RequestStatus.WithEmployerForApproval, LastAction.Approve, AgreementStatus.ProviderAgreed, TestName = "Approved and sent to employer")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus agreementStatus)
        {
            var commitment = new CommitmentListItem
            {
                LastAction = lastAction,
                AgreementStatus = agreementStatus,

                EditStatus = EditStatus.EmployerOnly,
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
        [TestCase(LastAction.Approve, AgreementStatus.ProviderAgreed)]
        public void WhenTheEmployerHasNeverModifiedTheCommitmentItIsClassedAsNew(LastAction lastAction, AgreementStatus agreementStatus)
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
