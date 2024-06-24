using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;
using System.Diagnostics;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.ScheduledJobs
{
    public class UpdateUsersFromDfeSignIn
    {
        private readonly IIdamsSyncService _idamsSyncService;
        private readonly ILogger<UpdateUsersFromDfeSignIn> _logger;

        public UpdateUsersFromDfeSignIn(IIdamsSyncService idamsSyncService, ILogger<UpdateUsersFromDfeSignIn> logger)
        {
            _idamsSyncService = idamsSyncService;
            _logger = logger;
        }

        public async Task UdateUserPreferencesJob([TimerTrigger("* */15 * * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            _logger.LogInformation("UpdateUsersFromDfESignIn job started");
            try
            {
                var timer = Stopwatch.StartNew();
                await _idamsSyncService.SyncUsers();
                timer.Stop();
                _logger.LogInformation(
                    $"UpdateUsersFromDfESignIn job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (AggregateException exc)
            {
                _logger.LogError(exc, "Error running UpdateUsersFromDfESignIn WebJob");
                exc.Handle(ex =>
                {
                    _logger.LogError(ex, "Inner exception running UpdateUsersFromDfESignIn WebJob");
                    return false;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running UpdateUsersFromDfESignIn WebJob");
                throw;
            }
        }
    }
}
