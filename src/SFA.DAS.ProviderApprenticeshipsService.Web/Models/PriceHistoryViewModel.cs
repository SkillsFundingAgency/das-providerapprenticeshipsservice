using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class PriceHistoryViewModel
    {
        public long ApprenticeshipId { get; set; }

        public decimal Cost { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}