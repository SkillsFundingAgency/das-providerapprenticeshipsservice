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
    public sealed class WhenEditStatusIsWithBoth
    {
        [TestCase(RequestStatus.Approved, LastAction.Approve, TestName = "Approved by both parties")]
        public void WhenThereAreNoApprentices(RequestStatus expectedResult, LastAction lastAction)
        {
            var commitment = new CommitmentListItem
            {
                LastAction = lastAction,

                EditStatus = EditStatus.Both,
                ApprenticeshipCount = 2,
                AgreementStatus = AgreementStatus.BothAgreed,
                ProviderLastUpdateInfo = new LastUpdateInfo()
            };

            var status = commitment.GetStatus();

            status.Should().Be(expectedResult);
        }
    }
}
