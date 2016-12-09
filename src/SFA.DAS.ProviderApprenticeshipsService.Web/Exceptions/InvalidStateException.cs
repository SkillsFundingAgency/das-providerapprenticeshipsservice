using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message) {}
    }
}
