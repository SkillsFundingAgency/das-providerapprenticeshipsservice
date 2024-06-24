using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ScheduledJobs
{
    public class UpdateAgreementStatusJob
    {
        private readonly IProviderAgreementStatusService _providerAgreementStatusService;
        private readonly ILogger<UpdateAgreementStatusJob> _logger;

        public UpdateAgreementStatusJob(IProviderAgreementStatusService providerAgreementStatusService, ILogger<UpdateAgreementStatusJob> logger)
        {
            _providerAgreementStatusService = providerAgreementStatusService;
            _logger = logger;
        }

        public async Task UpdateAgreementStatus([TimerTrigger("* */15 * * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            try
            {
                _logger.LogInformation("ContractAgreements job started");
                var timer = Stopwatch.StartNew();
                await _providerAgreementStatusService.UpdateProviderAgreementStatuses();
                timer.Stop();

                _logger.LogInformation($"ContractAgreements job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (AggregateException exc)
            {
                _logger.LogError(exc, "Error running ContractAgreements WebJob");
                exc.Handle(ex =>
                {
                    _logger.LogError(ex, "Inner exception running ContractAgreements WebJob");
                    return false;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running ContractAgreements WebJob");
                throw;
            }
        }
    }
}
