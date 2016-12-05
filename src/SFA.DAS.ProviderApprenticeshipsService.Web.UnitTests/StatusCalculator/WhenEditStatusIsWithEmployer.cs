using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithEmployer
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.NotAgreed, TestName = "Sent request for review to employer")]
        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.EmployerAgreed, TestName = "Sent back approved request for review")]
        [TestCase(RequestStatus.WithEmployerForApproval, LastAction.Approve, AgreementStatus.ProviderAgreed, TestName = "Approved and sent to employer")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            var status = _calculator.GetStatus(EditStatus.EmployerOnly, 2, lastAction, overallAgreementStatus);

            status.Should().Be(expectedResult);
        }
    }
}
