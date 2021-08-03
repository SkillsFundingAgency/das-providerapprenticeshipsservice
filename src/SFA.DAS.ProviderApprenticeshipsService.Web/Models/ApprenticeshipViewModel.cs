using System;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(ApprenticeshipViewModelValidator))]
    public class ApprenticeshipViewModel
    {
        private const int CurrentYearAsTwoDigitOffSet = 0;

        public ApprenticeshipViewModel()
        {
            StartDate = new DateTimeViewModel();
            EndDate = new DateTimeViewModel();
        }

        public string HashedApprenticeshipId { get; set; }

        public string HashedCommitmentId { get; set; }

        public long ProviderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTimeViewModel DateOfBirth { get; set; } = new DateTimeViewModel(CurrentYearAsTwoDigitOffSet);

        public string NINumber { get; set; }

        public string ULN { get; set; }

        public TrainingType CourseType { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Cost { get; set; }

        public DateTimeViewModel StartDate { get; set; }
        public DateTimeViewModel StopDate { get; set; }
        public DateTimeViewModel EndDate { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public AgreementStatus AgreementStatus { get; set; }
        public string ProviderRef { get; set; }
        public string EmployerRef { get; set; }

        public bool HasStarted { get; set; }

        public bool IsLockedForUpdate { get; set; }
        public bool IsPaidForByTransfer { get; set; }
        public bool IsUpdateLockedForStartDateAndCourse { get; set; }
        public bool IsEndDateLockedForUpdate { get; set; }
        public string StartDateTransfersMinDateAltDetailMessage { get; set; }
        public Guid? ReservationId { get; set; }
        public bool IsContinuation { get; set; }

        public string EmailAddress { get; set; }
        public string AgreementId { get; set; }
    }
}