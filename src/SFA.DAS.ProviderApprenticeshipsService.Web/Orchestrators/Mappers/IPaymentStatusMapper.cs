using System;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IPaymentStatusMapper
    {
        string Map(PaymentStatus paymentStatus, DateTime? startDate);
    }
}