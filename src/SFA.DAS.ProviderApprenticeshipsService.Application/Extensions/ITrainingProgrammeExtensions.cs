﻿using System;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Extensions
{
    public static class ITrainingProgrammeExtensions
    {
        public static bool IsActiveOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            return (!course.EffectiveFrom.HasValue || course.EffectiveFrom.Value.FirstOfMonth() <= effectiveDate.Date) &&
                   (!course.EffectiveTo.HasValue || course.EffectiveTo.Value.Date >= effectiveDate.FirstOfMonth());
        }

        public static int FundingCapOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            if (!course.IsActiveOn(effectiveDate))
            {
                return 0;
            }

            var applicableFundingPeriod = course.FundingPeriods.FirstOrDefault(x =>
                (!x.EffectiveFrom.HasValue || x.EffectiveFrom.Value.Date <= effectiveDate.Date) &&
                (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= effectiveDate.Date));

            return applicableFundingPeriod?.FundingCap ?? 0;
        }
    }
}
