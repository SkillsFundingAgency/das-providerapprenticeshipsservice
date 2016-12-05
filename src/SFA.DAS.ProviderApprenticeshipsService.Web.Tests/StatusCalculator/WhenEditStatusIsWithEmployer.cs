using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithEmployer
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        //[TestCase(RequestStatus.SentForReview, LastAction.Amend, TestName = "Empty Amend request sent to Employer")]
        //[TestCase(RequestStatus.ReadyForReview, LastAction.Amend, TestName = "Empty Amend request sent from Employer")]
        //[TestCase(RequestStatus.ReadyForReview, LastAction.Approve, TestName = "Empty Approve request sent from Employer")]
        //public void WhenThereAreNoApprentices(RequestStatus expectedResult, LastAction lastAction)
        //{
        //    var status = _calculator.GetStatus(EditStatus.EmployerOnly, 0, lastAction);

        //    status.Should().Be(expectedResult);
        //}

        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.NotAgreed, TestName = "None Agreed Amend request sent for review by provider")]
        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.EmployerAgreed, TestName = "Employer Agreed Amend request sent for review by provider")]
        [TestCase(RequestStatus.WithEmployerForApproval, LastAction.Approve, AgreementStatus.ProviderAgreed, TestName = "Provider Agreed Approve request sent for approval by employer")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            var status = _calculator.GetStatus(EditStatus.EmployerOnly, 2, lastAction, overallAgreementStatus);

            status.Should().Be(expectedResult);
        }
    }
}
