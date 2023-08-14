using System;
using System.Runtime.Serialization;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies;

[Serializable]
public class TooManyRequestsException : Exception
{
    public TooManyRequestsException() : base("Rate limit has been reached") { }

    protected TooManyRequestsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}