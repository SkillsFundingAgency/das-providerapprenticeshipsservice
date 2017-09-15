﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class AcademicYearDateProvider : IAcademicYearDateProvider
    {
        private readonly ICurrentDateTime _currentDateTime;

        public AcademicYearDateProvider(ICurrentDateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
        }

        public DateTime CurrentAcademicYearStartDate
        {
            get
            {
                var now = _currentDateTime.Now;

                var cutoff = new DateTime(now.Year, 8, 1);

                return now >= cutoff ? cutoff : new DateTime(now.Year - 1, 8, 1);
            }
        }

        public DateTime CurrentAcademicYearEndDate => CurrentAcademicYearStartDate.AddYears(1).AddDays(-1);

        public DateTime LastAcademicYearFundingPeriod
        {
            get
            {
                // TODO GET DATE FROM SOURCE
                return new DateTime(_currentDateTime.Now.Year, 10, 19, 18, 0, 0);
            }
        }

    }
}
