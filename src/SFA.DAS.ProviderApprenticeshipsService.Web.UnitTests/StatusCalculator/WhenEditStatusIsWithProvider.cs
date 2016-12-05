using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithProvider
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.NewRequest, LastAction.None, AgreementStatus.NotAgreed, TestName = "Empty or new request with new apprenticesships")]
        [TestCase(RequestStatus.ReadyForReview, LastAction.Amend, AgreementStatus.NotAgreed, TestName = "Request sent for review with no agreemeent from either party")]
        [TestCase(RequestStatus.ReadyForApproval, LastAction.Approve, AgreementStatus.EmployerAgreed, TestName = "Ready for approval")]
        [TestCase(RequestStatus.ReadyForReview, LastAction.Approve, AgreementStatus.NotAgreed, TestName = "Sent for approval but a change has been made")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            var status = _calculator.GetStatus(EditStatus.ProviderOnly, 2, lastAction, overallAgreementStatus);

            status.Should().Be(expectedResult);
        }
    }
}
