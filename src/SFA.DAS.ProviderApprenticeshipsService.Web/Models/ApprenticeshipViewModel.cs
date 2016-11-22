using System.Collections.Generic;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ExtendedApprenticeshipViewModel
    {
        public ApprenticeshipViewModel Apprenticeship { get; set; }
        public List<Domain.ITrainingProgramme> ApprenticeshipProgrammes { get; set; }
    }

    [Validator(typeof(ApprenticeshipViewModelValidator))]
    public class ApprenticeshipViewModel
    {
        public long Id { get; set; }
        public long CommitmentId { get; set; }
        public long ProviderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DateOfBirth { get; set; }

        public int? DateOfBirthDay { get; set; }

        public int? DateOfBirthMonth { get; set; }

        public int? DateOfBirthYear { get; set; }

        public string NINumber { get; set; }

        public string ULN { get; set; }
        public TrainingType TrainingType { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public string Cost { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public AgreementStatus AgreementStatus { get; set; }

        public string ProviderRef { get; set; }

        public string EmployerRef { get; set; }
    }
}