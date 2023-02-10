using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IAcademicYearDateProviderService
    {
        DateTime CurrentAcademicYearStartDate { get; }
        DateTime CurrentAcademicYearEndDate { get; }

        DateTime LastAcademicYearFundingPeriod { get; }
    }
}
