using System;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class PaymentStatusMapper : IPaymentStatusMapper
    {
        private readonly ICurrentDateTime _currentDateTime;

        public PaymentStatusMapper(ICurrentDateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
        }

        public string Map(PaymentStatus paymentStatus, DateTime? startDate)
        {
            var isStartDateInFuture = startDate.HasValue && startDate.Value > new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1);

            switch (paymentStatus)
            {
                case PaymentStatus.PendingApproval:
                    return "Approval needed";
                case PaymentStatus.Active:
                    return
                        isStartDateInFuture ? "Waiting to start" : "Live";
                case PaymentStatus.Paused:
                    return "Paused";
                case PaymentStatus.Withdrawn:
                    return "Stopped";
                case PaymentStatus.Completed:
                    return "Finished";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }
    }
}