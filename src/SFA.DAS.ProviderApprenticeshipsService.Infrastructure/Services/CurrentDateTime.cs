using System;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        private readonly DateTime? _time;

        //public DateTime Now => _time ?? DateTime.UtcNow;
        public DateTime Now => DateTime.Parse("20 OCT 2017");
        public CurrentDateTime()
        {
        }

        public CurrentDateTime(DateTime? time)
        {
            _time = time;
        }
    }
}
