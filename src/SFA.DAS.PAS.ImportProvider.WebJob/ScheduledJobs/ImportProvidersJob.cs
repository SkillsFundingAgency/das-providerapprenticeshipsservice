using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;

namespace SFA.DAS.PAS.ImportProvider.WebJob.ScheduledJobs
{
    public class ImportProvidersJob
    {
        private readonly IImportProviderService _importProviderService;
        private readonly ILogger<ImportProvidersJob> _logger;

        public ImportProvidersJob(IImportProviderService importProviderService, ILogger<ImportProvidersJob> logger)
        {
            _importProviderService = importProviderService;
            _logger = logger;
        }

        public async Task ImportProviders([TimerTrigger("* */15 * * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            try
            {
                _logger.LogInformation("ImportProvider job started");
                var timer = Stopwatch.StartNew();

                await _importProviderService.Import();

                timer.Stop();
                _logger.LogInformation($"ImportProvider job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (AggregateException exc)
            {
                _logger.LogError(exc, "Error running ImportProvider WebJob");
                exc.Handle(ex =>
                {
                    _logger.LogError(ex, "Inner exception running ImportProvider WebJob");
                    return false;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running ImportProvider WebJob");
                throw;
            }
        }
    }
}
