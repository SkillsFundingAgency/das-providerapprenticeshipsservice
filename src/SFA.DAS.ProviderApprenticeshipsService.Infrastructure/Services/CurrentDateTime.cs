using System;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
