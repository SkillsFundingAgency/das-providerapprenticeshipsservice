using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class CommitmentStatusCalculator : ICommitmentStatusCalculator
    {
        public RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            bool hasApprenticeships = apprenticeshipCount > 0;

            if (editStatus == EditStatus.Both)
                return RequestStatus.Approved;

            if (editStatus == EditStatus.ProviderOnly)
            {
                return GetProviderOnlyStatus(lastAction, hasApprenticeships, overallAgreementStatus);
            }

            if (editStatus == EditStatus.EmployerOnly)
            {
                return GetEmployerOnlyStatus(lastAction, hasApprenticeships);
            }

            return RequestStatus.None;
        }

        private static RequestStatus GetProviderOnlyStatus(LastAction lastAction, bool hasApprenticeships, AgreementStatus overallAgreementStatus)
        {
            if (!hasApprenticeships || lastAction == LastAction.None)
                return RequestStatus.NewRequest;

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

        private RequestStatus GetEmployerOnlyStatus(LastAction lastAction, bool hasApprenticeships)
        {
            if (!hasApprenticeships || lastAction == LastAction.None)
                return RequestStatus.None;

            if(lastAction == LastAction.Amend)
                return RequestStatus.SentForReview;

            if (lastAction == LastAction.Approve)
                return RequestStatus.WithEmployerForApproval;

            return RequestStatus.SentForReview;
        }
    }
}