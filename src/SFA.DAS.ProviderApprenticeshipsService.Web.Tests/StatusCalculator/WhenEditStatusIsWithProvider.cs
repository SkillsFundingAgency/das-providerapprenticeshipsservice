using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithProvider
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.NewRequest, LastAction.None, TestName = "New empty request from Employer")]
        //[TestCase(RequestStatus.ReadyForReview, LastAction.Amend, TestName = "Empty Amend request sent from Employer")]
        //[TestCase(RequestStatus.ReadyForReview, LastAction.Approve, TestName = "Empty Approve request sent from Employer")]
        public void WhenThereAreNoApprentices(RequestStatus expectedResult, LastAction lastAction)
        {
            var status = _calculator.GetStatus(EditStatus.ProviderOnly, 0, lastAction, AgreementStatus.NotAgreed);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.NewRequest, LastAction.None, AgreementStatus.NotAgreed, TestName = "Provider adds apprenticeship to new request")]
        [TestCase(RequestStatus.ReadyForReview, LastAction.Amend, AgreementStatus.NotAgreed, TestName = "None Agreed Amend request sent for review by employer")]
        //[TestCase(RequestStatus., LastAction.Amend, AgreementStatus.EmployerAgreed, TestName = "Employer Agreed Amend request sent for review by employer")]
        //[TestCase(RequestStatus., LastAction.Amend, AgreementStatus.BothAgreed, TestName = "Both Agreed Amend request sent for review by employer")]
        [TestCase(RequestStatus.ReadyForApproval, LastAction.Approve, AgreementStatus.EmployerAgreed, TestName = "Employer Agreed Approve request sent for approval by employer")]
        [TestCase(RequestStatus.ReadyForReview, LastAction.Approve, AgreementStatus.NotAgreed, TestName = "None Agreed Approve request sent for approval by employer")]
        //[TestCase(RequestStatus., LastAction.Approve, AgreementStatus.BothAgreed, TestName = "None Agreed Approve request sent for approval by employer")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            var status = _calculator.GetStatus(EditStatus.ProviderOnly, 2, lastAction, overallAgreementStatus);

            status.Should().Be(expectedResult);
        }
    }
}
