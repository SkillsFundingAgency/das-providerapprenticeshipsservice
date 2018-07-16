using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipListItemGroupViewModel
    {
        public ITrainingProgramme TrainingProgramme { get; set; }

        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; set; }

        public string GroupId => TrainingProgramme == null ? "0" : TrainingProgramme.Id;

        public string GroupName => TrainingProgramme == null ? "No training course" : TrainingProgramme.Title;
        public int ApprenticeshipsOverFundingLimit
        {
            get
            {
                return TrainingProgramme == null ? 0 : Apprenticeships.Count(x => x.Cost > TrainingProgramme.MaxFunding);
            }
        }

        public int OverlapErrorCount
        {
            get
            {
                return Apprenticeships.Count(x => x.OverlappingApprenticeships.Any());
            }
        }
     
    }
}