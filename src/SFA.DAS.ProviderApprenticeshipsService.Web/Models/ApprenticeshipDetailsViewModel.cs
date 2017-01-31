using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipDetailsViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public long Id { get; set; }

        public string Uln { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TrainingName { get; set; }

        public decimal? Cost { get; set; }

        public string Status { get; set; }

        public string EmployerName { get; set; }
    }
}