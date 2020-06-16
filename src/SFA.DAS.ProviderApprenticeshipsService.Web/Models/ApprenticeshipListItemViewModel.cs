using System;
using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class ApprenticeshipListItemViewModel
    {
        public long ApprenticeshipId { get; internal set; }
        public string HashedApprenticeshipId { get; set; }
        public string ApprenticeshipName { get; internal set; }
        public DateTime? ApprenticeDateOfBirth { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? EndDate { get; internal set; }
        public DateTime? StartDate { get; set; }
        public string CourseCode { get; internal set; }
        public string CourseName { get; internal set; }
        public string ULN { get; internal set; }
        public bool CanBeApproved { get; internal set; }
        public DateTime? OriginalStartDate { get; set; }
       
        public IEnumerable<OverlappingApprenticeship> OverlappingApprenticeships { get; set; }

        public bool IsOverFundingLimit(ITrainingProgramme trainingProgramme, bool isLinkedToChangeOfPartyRequest = false)
        {
            if (trainingProgramme == null)
                return false;

            if (!isLinkedToChangeOfPartyRequest && !StartDate.HasValue)
                return false;

            var fundingCapAtStartDate = trainingProgramme.FundingCapOn(isLinkedToChangeOfPartyRequest ? OriginalStartDate.Value : StartDate.Value);
                return Cost.HasValue && fundingCapAtStartDate > 0
                                     && Cost > fundingCapAtStartDate;
        }
    }
}