using System;
using System.Collections.Generic;
using NLog;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging
{
    public sealed class ProviderCommitmentsLogger : IProviderCommitmentsLogger
    {
        private readonly ILogger _logger;

        public ProviderCommitmentsLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Trace(string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?))
        {
            IDictionary<string, object> properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
            _logger.Trace(message, properties);
        }

        public void Debug(string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?))
        {
            IDictionary<string, object> properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
            _logger.Debug(message, properties);
        }

        public void Info(string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?))
        {
            IDictionary<string, object> properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
            _logger.Info(message, properties);
        }

        public void Warn(string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?))
        {
            IDictionary<string, object> properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
            _logger.Warn(message, properties);
        }

        public void Warn(Exception ex, string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?))
        {
            IDictionary<string, object> properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
            _logger.Warn(ex, message, properties);
        }

        public void Error(Exception ex, string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?))
        {
            IDictionary<string, object> properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
            _logger.Error(ex, message, properties);
        }

        public void Fatal(Exception ex, string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?))
        {
            IDictionary<string, object> properties = BuildPropertyDictionary(providerId, commitmentId, apprenticeshipId);
            _logger.Fatal(ex, message, properties);
        }

        private IDictionary<string, object> BuildPropertyDictionary(long? providerId, long? commitmentId, long? apprenticeshipId)
        {
            var properties = new Dictionary<string, object>();

            if (providerId.HasValue) properties.Add("ProviderId", providerId.Value);
            if (commitmentId.HasValue) properties.Add("CommitmentId", commitmentId.Value);
            if (apprenticeshipId.HasValue) properties.Add("ApprenticeshipId", apprenticeshipId.Value);

            return properties;
        }
    }
}
