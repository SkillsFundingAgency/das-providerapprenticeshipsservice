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

        public DateTimeViewModel DateOfBirth { get; set; }

        public string ULN { get; set; }

        public TrainingType? TrainingType { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public string Cost { get; set; }

        public DateTimeViewModel StartDate { get; set; }

        public DateTimeViewModel EndDate { get; set; }

        public string ProviderRef { get; set; }
        public string EmployerRef { get; set; }

        public Apprenticeship OriginalApprenticeship { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
    }
}