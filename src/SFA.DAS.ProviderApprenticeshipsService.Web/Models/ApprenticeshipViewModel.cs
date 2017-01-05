using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(ApprenticeshipViewModelValidator))]
    public class ApprenticeshipViewModel
    {
        private const int CurrentYearAsTwoDigitOffSet = 0;

        public string HashedApprenticeshipId { get; set; }

        public string HashedCommitmentId { get; set; }

        public long ProviderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTimeViewModel DateOfBirth { get; set; } = new DateTimeViewModel(CurrentYearAsTwoDigitOffSet);

        public string NINumber { get; set; }
        public string ULN { get; set; }

        public TrainingType TrainingType { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public string Cost { get; set; }

        public DateTimeViewModel StartDate { get; set; }

        public DateTimeViewModel EndDate { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public AgreementStatus AgreementStatus { get; set; }
        public string ProviderRef { get; set; }
        public string EmployerRef { get; set; }   
    }
}