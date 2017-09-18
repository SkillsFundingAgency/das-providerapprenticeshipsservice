using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(FinishEditingViewModelValidator))]
    public sealed class FinishEditingViewModel
    {
        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public bool HasApprenticeships { get; set; }

        public int InvalidApprenticeshipCount { get; set; }

        public SaveStatus SaveStatus { get; set; }

        public ApprovalState ApprovalState { get; set; }

        public bool ReadyForApproval { get; set; }

        public bool IsApproveAndSend => ApprovalState == ApprovalState.ApproveAndSend;

        public bool HasSignedTheAgreement { get; set; }

        public string SignAgreementUrl { get; set; }

        public bool HasOverlappingErrors { get; set; }

        public bool HasAcademicFundingPeriodErrors { get; set; }

        public bool CanApprove
        {
            get
            {
                return ReadyForApproval &&
                    HasSignedTheAgreement &&
                    !HasOverlappingErrors &&
                    !HasAcademicFundingPeriodErrors;
            }

        }
    }

    public enum ApprovalState
    {
        ApproveAndSend = 0,
        ApproveOnly = 1
    }
}
