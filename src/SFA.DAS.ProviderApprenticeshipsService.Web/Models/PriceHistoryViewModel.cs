using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class PriceHistoryViewModel
    {
        public long ApprenticeshipId { get; set; }

        public decimal Cost { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime? IlrEffectiveFromDate { get; set; }

        public DateTime? IlrEffectiveToDate { get; set; }

        public decimal? IlrTotalCost { get; set; }

    }
}