using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

public interface ICurrentDateTime
{
    DateTime Now { get; }
}