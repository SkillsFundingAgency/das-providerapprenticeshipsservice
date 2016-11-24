using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests
{
    [TestFixture]
    public sealed class WhenGettingStatusOfCommitment
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.NewRequest, EditStatus.ProviderOnly, 0, null, TestName = "With Provider, no apprenticesips")]
        [TestCase(RequestStatus.SentToEmployer, EditStatus.EmployerOnly, 0, null, TestName = "With Employer, no apprenticesips")]
        // TODO: this needs to differentiate when sent from employer for ammendment or provider adds to empty commitment
        [TestCase(RequestStatus.NewRequest, EditStatus.ProviderOnly, 2, AgreementStatus.NotAgreed, TestName = "With Provider, all apprenticeships NotAgreed")]
        [TestCase(RequestStatus.SentToEmployer, EditStatus.EmployerOnly, 2, AgreementStatus.NotAgreed, TestName = "With Employer, all apprenticeships NotAgreed")]
        [TestCase(RequestStatus.ReadyForApproval, EditStatus.ProviderOnly, 2, AgreementStatus.EmployerAgreed, TestName = "With Provider, all apprenticeships EmployerAgreed")]
        [TestCase(RequestStatus.SentToEmployer, EditStatus.EmployerOnly, 2, AgreementStatus.EmployerAgreed, TestName = "With Employer, all apprenticeships EmployerAgreed")]
        [TestCase(RequestStatus.WithEmployerForApproval, EditStatus.EmployerOnly, 2, AgreementStatus.ProviderAgreed, TestName = "With Employer, all apprenticeships ProviderAgreed")]
        [TestCase(RequestStatus.Approved, EditStatus.Both, 2, AgreementStatus.BothAgreed, TestName = "With Both, all apprenticeships BothAgreed")]
        public void ThenReturnsStatusBasedOnEditStatusAndApprenticeships(RequestStatus expectedResult, EditStatus editStatus, int numberOfApprenticeships, AgreementStatus? overallAgreementStatus)
        {
            var status = _calculator.GetStatus(editStatus, numberOfApprenticeships, overallAgreementStatus);

            status.Should().Be(expectedResult);
        }
    }
}
