using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(FinishEditingViewModelValidator))]
    public sealed class FinishEditingViewModel
    {
        public FinishEditingViewModel() { }

        public FinishEditingViewModel(string hashedCommitmentId, Commitment commitment)
        {
            HashedCommitmentId = hashedCommitmentId;
            ProviderId = commitment.ProviderId.Value;
            State = GetOptionsState(commitment);
        }

        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public SaveStatus SaveStatus { get; set; }

        public OptionsState State { get; set; }

        public string Message { get; set; }

        private static OptionsState GetOptionsState(Commitment commitment)
        {
            if (!commitment.CanBeApproved)
            {
                var message = GetInvalidStateForApprovalMessage(commitment);

                return new OptionsState
                {
                    ApprovalState = ApprovalState.NotReadyForApproval,
                    Message = message
                };
            }

            var approvalState = commitment.Apprenticeships.Any(m => m.AgreementStatus == AgreementStatus.NotAgreed
                                   || m.AgreementStatus == AgreementStatus.ProviderAgreed) ? ApprovalState.ApproveAndSend : ApprovalState.ApproveOnly;

            return new OptionsState { ApprovalState = approvalState };
        }

        private static string GetInvalidStateForApprovalMessage(Commitment commitment)
        {
            if (commitment.Apprenticeships.Count == 0)
                return "There needs to be at least 1 apprentice in a cohort";

            var invalidCount = commitment.Apprenticeships.Count(x => x.CanBeApproved == false);

            return invalidCount == 1
                ? "There is 1 apprentice that has incomplete details"
                : $"There are {invalidCount} apprentices that have incomplete details";
        }
    }

    public class OptionsState
    {
        public ApprovalState ApprovalState { get; set; }
        public string Message { get; set; }
    }

    public enum ApprovalState
    {
        NotReadyForApproval = 0,
        ApproveAndSend = 1,
        ApproveOnly = 2
    }
}
