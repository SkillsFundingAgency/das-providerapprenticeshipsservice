using System;
using System.Runtime.Serialization;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies;

[Serializable]
public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException() : base("Service is unavailable") { }

    protected ServiceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}