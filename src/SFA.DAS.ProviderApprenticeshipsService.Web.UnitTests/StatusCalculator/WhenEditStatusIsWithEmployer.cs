using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithEmployer
    {
        private static readonly CommitmentStatusCalculator Calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.NotAgreed, TestName = "Sent request for review to employer")]
        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.EmployerAgreed, TestName = "Sent back approved request for review")]
        [TestCase(RequestStatus.WithEmployerForApproval, LastAction.Approve, AgreementStatus.ProviderAgreed, TestName = "Approved and sent to employer")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            var lastUpdateInfo = new LastUpdateInfo
            {
                EmailAddress = "test@testcorp",
                Name = "Test"
            };

            var status = Calculator.GetStatus(EditStatus.EmployerOnly, 2, lastAction, overallAgreementStatus, lastUpdateInfo, null, null);

            status.Should().Be(expectedResult);
        }


        [TestCase(LastAction.None, AgreementStatus.NotAgreed)]
        [TestCase(LastAction.Amend, AgreementStatus.NotAgreed)]
        [TestCase(LastAction.Approve, AgreementStatus.ProviderAgreed)]
        public void WhenTheEmployerHasNeverModifiedTheCommitmentItIsClassedAsNew(LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            //Act
            var status = Calculator.GetStatus(EditStatus.ProviderOnly, 2, lastAction, overallAgreementStatus, null, null, null);

            //Assert
            Assert.AreEqual(RequestStatus.NewRequest, status);
        }
    }
}
