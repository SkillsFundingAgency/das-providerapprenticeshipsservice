using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithBoth
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.Approved, LastAction.Approve, TestName = "Approved by both parties")]
        public void WhenThereAreNoApprentices(RequestStatus expectedResult, LastAction lastAction)
        {
            var lastUpdateInfo = new LastUpdateInfo();

            var status = _calculator.GetStatus(EditStatus.Both, 2, lastAction, AgreementStatus.BothAgreed, lastUpdateInfo);

            status.Should().Be(expectedResult);
        }
    }
}
