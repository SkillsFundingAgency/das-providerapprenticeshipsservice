using System;
using System.Runtime.Serialization;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;

[Serializable]
public class InvalidConfigurationValueException: Exception
{
    public InvalidConfigurationValueException(string configurationItem) : base($"Configuration value for '{configurationItem}' is not valid.") { }

    protected InvalidConfigurationValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}