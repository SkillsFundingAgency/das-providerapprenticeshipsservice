using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging
{
    public interface IProviderCommitmentsLogger
    {
        void Trace(string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default);
        void Debug(string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default);
        void Info(string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default);
        void Warn(string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default);
        void Warn(Exception ex, string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default);
        void Error(Exception ex, string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default);
        void Critical(Exception ex, string message, long? providerId = default, long? commitmentId = default, long? apprenticeshipId = default);
    }
}
