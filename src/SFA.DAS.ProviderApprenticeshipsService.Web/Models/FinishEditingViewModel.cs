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

        public SaveStatus SaveStatus { get; set; }

        public string Message { get; set; }

        public ApprovalState ApprovalState { get; internal set; }

        public bool NotReadyForApproval { get; internal set; }

        public bool IsApproveAndSend => ApprovalState == ApprovalState.ApproveAndSend;
    }

    public enum ApprovalState
    {
        ApproveAndSend = 0,
        ApproveOnly = 1
    }
}
