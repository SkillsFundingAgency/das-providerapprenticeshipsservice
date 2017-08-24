using System;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        private readonly DateTime? _time;

        public DateTime Now => _time ?? DateTime.UtcNow;

        //todo: remove this constructor
        public CurrentDateTime() : this(null)
        {
        }

        public CurrentDateTime(DateTime? time)
        {
            _time = time;
        }
    }
}
