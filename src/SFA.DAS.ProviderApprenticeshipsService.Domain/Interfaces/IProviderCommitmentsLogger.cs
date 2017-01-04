using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderCommitmentsLogger
    {
        void Trace(string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?));
        void Debug(string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?));
        void Info(string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?));
        void Warn(string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?));
        void Warn(Exception ex, string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?));
        void Error(Exception ex, string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?));
        void Fatal(Exception ex, string message, long? providerId = default(long?), long? commitmentId = default(long?), long? apprenticeshipId = default(long?));
    }
}
