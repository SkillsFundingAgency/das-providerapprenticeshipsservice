using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class CommitmentStatusCalculator : ICommitmentStatusCalculator
    {
        public RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, LastAction lastAction, AgreementStatus overallAgreementStatus, LastUpdateInfo providerLastUpdateInfo)
        {
            if (editStatus == EditStatus.Both)
                return RequestStatus.Approved;

            if (string.IsNullOrWhiteSpace(providerLastUpdateInfo?.Name))
            {
                return RequestStatus.NewRequest;
            }

            if (editStatus == EditStatus.ProviderOnly)
            {
                return GetProviderOnlyStatus(lastAction, overallAgreementStatus);
            }

            if (editStatus == EditStatus.EmployerOnly)
            {
                return GetEmployerOnlyStatus(lastAction);
            }

            return RequestStatus.None;
        }

        private static RequestStatus GetProviderOnlyStatus(LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            if (lastAction == LastAction.None)
            {
                return RequestStatus.ReadyForReview;
            }

            if (lastAction == LastAction.Amend)
                return RequestStatus.ReadyForReview;

            if (lastAction == LastAction.Approve)
            {
                if (overallAgreementStatus == AgreementStatus.NotAgreed)
                    return RequestStatus.ReadyForReview;
                else
                    return RequestStatus.ReadyForApproval;
            }

            return RequestStatus.None;
        }

        private RequestStatus GetEmployerOnlyStatus(LastAction lastAction)
        {
            if (lastAction == LastAction.None)
                return RequestStatus.None;

            if(lastAction == LastAction.Amend)
                return RequestStatus.SentForReview;

            if (lastAction == LastAction.Approve)
                return RequestStatus.WithEmployerForApproval;

            return RequestStatus.SentForReview;
        }
    }
}