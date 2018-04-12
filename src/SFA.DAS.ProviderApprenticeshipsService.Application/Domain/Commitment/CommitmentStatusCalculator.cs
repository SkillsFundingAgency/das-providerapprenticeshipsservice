using System;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment
{
    public sealed class CommitmentStatusCalculator
    {
        /// <remarks>
        /// Note: any commitments with CommitmentStatus == Active are filtered out before GetStatus is called on them
        /// </remarks>
        public RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, LastAction lastAction,
            AgreementStatus overallAgreementStatus, LastUpdateInfo providerLastUpdateInfo,
            long? transferSenderId, TransferApprovalStatus? transferApprovalStatus)
        {
            if (transferSenderId.HasValue)
            {
                if (!transferApprovalStatus.HasValue)
                    throw new InvalidStateException("TransferSenderId supplied, but no TransferApprovalStatus");
                return GetTransferStatus(editStatus, transferApprovalStatus.Value, lastAction, overallAgreementStatus);
            }

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

        private RequestStatus GetTransferStatus(EditStatus edit, TransferApprovalStatus transferApproval, LastAction lastAction, AgreementStatus agreementStatus)
        {
            const string invalidStateExceptionMessagePrefix = "Transfer funder commitment in invalid state: ";

            if (edit >= EditStatus.Neither)
                throw new Exception("Unexpected EditStatus");

            switch (transferApproval)
            {
                case TransferApprovalStatus.Pending:
                    switch (edit)
                    {
                        case EditStatus.Both:
                            return RequestStatus.WithSenderForApproval;
                        case EditStatus.EmployerOnly:
                            return GetEmployerOnlyStatus(lastAction);
                        case EditStatus.ProviderOnly:
                            return GetProviderOnlyStatus(lastAction, agreementStatus);
                        default:
                            throw new Exception("Unexpected EditStatus");

                    }

                case TransferApprovalStatus.Approved:
                    if (edit != EditStatus.Both)
                        throw new InvalidStateException($"{invalidStateExceptionMessagePrefix}If approved by sender, must be approved by receiver and provider");
                    return RequestStatus.None;

                case TransferApprovalStatus.Rejected:
                    if (edit != EditStatus.EmployerOnly)
                        throw new InvalidStateException($"{invalidStateExceptionMessagePrefix}If just rejected by sender, must be with receiver");
                    return RequestStatus.RejectedBySender;

                default:
                    throw new Exception("Unexpected TransferApprovalStatus");
            }
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