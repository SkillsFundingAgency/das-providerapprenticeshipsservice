using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message) {}
    }
}
