using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;

public sealed class ProviderCommitmentsLogger : IProviderCommitmentsLogger
{
    private readonly ILogger<ProviderCommitmentsLogger> _logger;

    public ProviderCommitmentsLogger(ILogger<ProviderCommitmentsLogger> logger)
    {
        _logger = logger;
    }

    public void Trace(string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default)
    {
        var properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
        _logger.LogTrace(message, properties);
    }

    public void Debug(string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default)
    {
        var properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
        _logger.LogDebug(message, properties);
    }

    public void Info(string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default)
    {
        var properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
        _logger.LogInformation(message, properties);
    }

    public void Warn(string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default)
    {
        var properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
        _logger.LogWarning(message, properties);
    }

    public void Warn(Exception ex, string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default)
    {
        var properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
        _logger.LogWarning(ex, message, properties);
    }

    public void Error(Exception ex, string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default)
    {
        var properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
        _logger.LogError(ex, message, properties);
    }

    public void Critical(Exception ex, string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default)
    {
        var properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
        _logger.LogCritical(ex, message, properties);
    }

    private static IDictionary<string, object> BuildPropertyDictionary(long? providerId, long? commitmentId, long? apprenticeshipId)
    {
        var properties = new Dictionary<string, object>();

        if (providerId.HasValue) properties.Add("ProviderId", providerId.Value);
        if (commitmentId.HasValue) properties.Add("CommitmentId", commitmentId.Value);
        if (apprenticeshipId.HasValue) properties.Add("ApprenticeshipId", apprenticeshipId.Value);

        return properties;
    }
}