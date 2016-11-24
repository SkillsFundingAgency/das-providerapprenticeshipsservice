using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class CommitmentStatusCalculator : ICommitmentStatusCalculator
    {
        public RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, AgreementStatus? overallAgreementStatus)
        {
            bool hasApprenticeships = apprenticeshipCount > 0;

            if (editStatus == EditStatus.Both)
                return RequestStatus.Approved;

            if (editStatus == EditStatus.ProviderOnly)
            {

                if (hasApprenticeships && overallAgreementStatus == AgreementStatus.NotAgreed)
                    return RequestStatus.NewRequest;

                if (hasApprenticeships && overallAgreementStatus == AgreementStatus.EmployerAgreed)
                    return RequestStatus.ReadyForApproval;

                return RequestStatus.NewRequest;
            }

            if (hasApprenticeships && overallAgreementStatus == AgreementStatus.ProviderAgreed)
                return RequestStatus.WithEmployerForApproval;

            return RequestStatus.SentToEmployer;
        }
    }
}