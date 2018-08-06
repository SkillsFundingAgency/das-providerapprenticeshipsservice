﻿using System;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Extensions
{
    public static class ITrainingProgrammeExtensions
    {
        public static bool IsActiveOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            if ((!course.EffectiveFrom.HasValue || course.EffectiveFrom.Value <= effectiveDate) &&
                (!course.EffectiveTo.HasValue || course.EffectiveTo.Value >= effectiveDate)) return true;

            return false;
        }
    }
}
