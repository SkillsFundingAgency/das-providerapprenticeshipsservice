using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderDetails;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services
{
    /// <inheritdoc />
    public class TrainingProviderService : ITrainingProviderService
    {
        private readonly IRecruitApiClient _recruitApiClient;
        private readonly IProviderCommitmentsLogger _logger;

        public TrainingProviderService(IRecruitApiClient recruitApiClient, IProviderCommitmentsLogger logger)
        {
            _recruitApiClient = recruitApiClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<GetProviderResponseItem> GetProviderDetails(long ukprn)
        {
            _logger.Trace("Getting Provider Details from Outer Api");

            var retryPolicy = GetApiRetryPolicy();

            var result = await retryPolicy.Execute(
                context => _recruitApiClient.Get<GetProviderResponseItem>(new GetProviderRequest(ukprn)),
                new Dictionary<string, object>() {{"apiCall", "Providers"}});

            return result;
        }

        private Polly.Retry.RetryPolicy GetApiRetryPolicy()
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(4)
                    },
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.Warn(
                            $"Error connecting to Outer Api for {context["apiCall"]}. Retrying in {timeSpan.Seconds} secs...attempt: {retryCount}");
                    });
        }
    }
}
