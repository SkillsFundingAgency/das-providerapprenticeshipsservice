using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

public interface IAcademicYearDateProviderService
{
    DateTime CurrentAcademicYearStartDate { get; }
    DateTime CurrentAcademicYearEndDate { get; }

    DateTime LastAcademicYearFundingPeriod { get; }
}