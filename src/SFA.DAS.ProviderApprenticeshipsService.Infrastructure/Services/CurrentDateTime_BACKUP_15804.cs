using System;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
<<<<<<< HEAD
       public DateTime Now => DateTime.UtcNow;
=======
        private readonly DateTime? _time;

        public DateTime Now => _time ?? DateTime.UtcNow;

        public CurrentDateTime()
        {
        }

        public CurrentDateTime(DateTime? time)
        {
            _time = time;
        }
>>>>>>> master
    }
}
