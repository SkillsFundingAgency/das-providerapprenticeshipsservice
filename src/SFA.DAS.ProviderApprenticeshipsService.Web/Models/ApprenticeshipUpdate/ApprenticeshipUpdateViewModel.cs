using System;
using System.ComponentModel.DataAnnotations;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate
{
    public class ApprenticeshipUpdateViewModel
    {
        public string HashedApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTimeViewModel DateOfBirth { get; set; } = new DateTimeViewModel(0);

        public string ULN { get; set; }

        public TrainingType? CourseType { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Cost { get; set; }

        public DateTimeViewModel StartDate { get; set; }
        public DateTimeViewModel EndDate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ProviderRef { get; set; }
        public string EmployerRef { get; set; }

        public Apprenticeship OriginalApprenticeship { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public Guid? ReservationId { get; set; }
    }
}