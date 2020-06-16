using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipListItemGroupViewModel
    {
        public ITrainingProgramme TrainingProgramme { get; }
        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; }

        public int ApprenticeshipsOverFundingLimit { get; }
        public int? CommonFundingCap { get; }

        private bool AllApprenticeshipsOverFundingLimit =>
            Apprenticeships.Any() && ApprenticeshipsOverFundingLimit == Apprenticeships.Count;

        public bool ShowCommonFundingCap => AllApprenticeshipsOverFundingLimit && CommonFundingCap != null;

        public string GroupId => TrainingProgramme?.Id ?? "0";

        public string GroupName => TrainingProgramme?.Title ?? "No training course";

        public int OverlapErrorCount => Apprenticeships.Count(x => x.OverlappingApprenticeships.Any());
        public bool IsLinkedToChangeOfPartyRequest { get; set; }

        /// <remarks>
        /// ApprenticeshipsOverFundingLimit and CommonFundingCap are only guaraneteed to be correct if the ctor's params are not mutated after instantiation or on another thread during contruction
        /// </remarks>
        public ApprenticeshipListItemGroupViewModel(IList<ApprenticeshipListItemViewModel> apprenticeships, ITrainingProgramme trainingProgramme = null, bool isLinkedToChangeOfPartyRequest = false)
        {
            TrainingProgramme = trainingProgramme;
            Apprenticeships = apprenticeships;
            IsLinkedToChangeOfPartyRequest = isLinkedToChangeOfPartyRequest;

            // calculating up-front assumes apprenticeships list and training program are not mutated after being passed to ctor
            ApprenticeshipsOverFundingLimit = CalculateApprenticeshipsOverFundingLimit();
            CommonFundingCap = CalculateCommonFundingCap();
        }

        /// <remarks>
        /// if the training program is not effective on the start date, the user will get a validation message when creating the apprenticeship
        /// (e.g. This training course is only available to apprentices with a start date after 04 2018)
        /// so we shouldn't see FundingCapOn returning 0 (when the start date is outside of a funding cap)
        /// but if we see it, we treat the apprenticeship as *not* over the funding limit
        /// </remarks>
        private int CalculateApprenticeshipsOverFundingLimit()
        {
            if (TrainingProgramme == null)
                return 0;

            return Apprenticeships.Count(x => x.IsOverFundingLimit(TrainingProgramme, IsLinkedToChangeOfPartyRequest));
        }

        /// <summary>
        /// If all apprenticeships share the same Funding Cap, this is it.
        /// If they have different funding caps, or there is no trainingprogram or apprenticeships,
        /// or there is not enough data to calculate the funding cap for each apprenticeship, this is null
        /// </summary>
        private int? CalculateCommonFundingCap()
        {
            if (TrainingProgramme == null || !Apprenticeships.Any())
                return null;

            if (!IsLinkedToChangeOfPartyRequest && Apprenticeships.Any(a => !a.StartDate.HasValue))
                return null;

            int firstFundingCap = IsLinkedToChangeOfPartyRequest 
                          ? TrainingProgramme.FundingCapOn(Apprenticeships.First().OriginalStartDate.Value)
                          : TrainingProgramme.FundingCapOn(Apprenticeships.First().StartDate.Value);

            // check for magic 0, which means unable to calculate a funding cap (e.g. date out of bounds)
            if (firstFundingCap == 0)
                return null;
            
            if (Apprenticeships.Skip(1).Any(a => TrainingProgramme.FundingCapOn(a.StartDate.Value) != firstFundingCap))
                return null;

            return firstFundingCap;
        }
    }
}