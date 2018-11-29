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
        [TestCase(LastAction.None, AgreementStatus.NotAgreed, RequestStatus.NewRequest, Description = "Provider draft")]
        [TestCase(LastAction.Amend, AgreementStatus.NotAgreed, RequestStatus.ReadyForReview)]
        [TestCase(LastAction.Approve, AgreementStatus.EmployerAgreed, RequestStatus.ReadyForApproval)]
        [TestCase(LastAction.Approve, AgreementStatus.NotAgreed, RequestStatus.ReadyForReview, Description = "Sent for approval but a change has been made")]
        public void ThenTheRequestHasTheCorrectStatus(LastAction lastAction, AgreementStatus agreementStatus, RequestStatus expectRequestStatus)
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
            Assert.AreEqual(expectRequestStatus, status);
        }
    }
}
