using System.Collections.Generic;
using FluentValidation.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(DeleteCohortConfirmationViewModelValidator))]
    public sealed class DeleteCommitmentViewModel
    {
        public string LegalEntityName { get; set; }

        public string CohortReference { get; set; }

        public int NumberOfApprenticeships { get; set; }

        public List<string> ApprenticeshipTrainingProgrammes { get; set; }

        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public bool? DeleteConfirmed { get; set; }
    }
}