using System;
using Microsoft.Extensions.Logging;
using Polly;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies
{
    [PolicyName(Name)]
    public class IdamsExecutionPolicy : ExecutionPolicy
    {
        private readonly ILogger<IdamsExecutionPolicy> _logger;

        public const string Name = "IDAMS Policy";

        public IdamsExecutionPolicy(ILogger<IdamsExecutionPolicy> logger)
        {
            _logger = logger;

            var tooManyRequestsPolicy = CreateAsyncRetryPolicy<TooManyRequestsException>(9, new TimeSpan(0, 0, 1), OnRetryableFailure);
            var serviceUnavailablePolicy = CreateAsyncRetryPolicy<ServiceUnavailableException>(4, new TimeSpan(0, 0, 5), OnRetryableFailure);
            RootPolicy = Policy.Wrap(tooManyRequestsPolicy, serviceUnavailablePolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            _logger.LogError(ex, $"Exceeded retry limit - {ex.Message}");
            return default(T);
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.LogInformation($"Error calling IDAMS - {ex.Message} - Will retry");
        }
    }
}
