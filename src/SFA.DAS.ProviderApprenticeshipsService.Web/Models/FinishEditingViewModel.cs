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

        public bool ApproveAndSend { get; set; }
    }
}