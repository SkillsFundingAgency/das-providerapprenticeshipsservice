using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class AlertMapper : IAlertMapper
    {
        public List<string> MapAlerts(Apprenticeship apprenticeship)
        {
            var result = new List<string>();

            if (apprenticeship.DataLockCourse || apprenticeship.DataLockPrice)
            {
                result.Add("ILR data mismatch");
            }

            if (apprenticeship.DataLockPriceTriaged || apprenticeship.DataLockCourseChangeTriaged)
            {
                result.Add("Changes pending");
            }

            if (apprenticeship.DataLockCourseTriaged)
            {
                result.Add("Changes requested");
            }

            if (apprenticeship.PendingUpdateOriginator != null)
            {
                if (apprenticeship.PendingUpdateOriginator == Originator.Provider)
                {
                    result.Add("Changes pending");
                }
                else
                {
                    result.Add("Changes for review");
                }
            }

            return result.Distinct().ToList();
        }

        public List<string> MapAlerts(ApprovedApprenticeshipViewModel model, ApprovedApprenticeship apprenticeship)
        {
            var result = new List<string>();

            if (model.DataLockCourse || model.DataLockPrice)
            {
                result.Add("ILR data mismatch");
            }

            if (model.DataLockPriceTriaged || model.DataLockCourseChangeTriaged)
            {
                result.Add("Changes pending");
            }

            if (model.DataLockCourseTriaged)
            {
                result.Add("Changes requested");
            }

            if (apprenticeship.UpdateOriginator != null)
            {
                if (apprenticeship.UpdateOriginator == Originator.Provider)
                {
                    result.Add("Changes pending");
                }
                else
                {
                    result.Add("Changes for review");
                }
            }

            return result.Distinct().ToList();
        }
    }
}