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
        [TestCase(LastAction.None, AgreementStatus.NotAgreed, RequestStatus.None, Description = "Employer draft not visible to provider")]
        [TestCase(LastAction.Amend, AgreementStatus.NotAgreed, RequestStatus.SentForReview)]
        [TestCase(LastAction.Approve, AgreementStatus.ProviderAgreed, RequestStatus.WithEmployerForApproval)]
        [TestCase(LastAction.Amend, AgreementStatus.EmployerAgreed, RequestStatus.SentForReview, TestName = "Sent back approved request for review")]
        public void ThenTheRequestHasTheCorrectStatus(LastAction lastAction, AgreementStatus agreementStatus, RequestStatus expectRequestStatus)
        {
            var commitment = new CommitmentListItem
            {
                LastAction = lastAction,
                AgreementStatus = agreementStatus,
                EditStatus = EditStatus.EmployerOnly,
                ApprenticeshipCount = 2
            };

            //Act
            var status = commitment.GetStatus();

            //Assert
            Assert.AreEqual(expectRequestStatus, status);
        }
    }
}
