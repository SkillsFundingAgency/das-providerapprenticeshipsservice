using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;
using Microsoft.Extensions.Hosting;
using NLog;
using System.Diagnostics;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.ScheduledJobs
{
    public class UpdateUserPreferences
    {
        private readonly IIdamsSyncService _idamsSyncService;
        private readonly ILogger<UpdateUserPreferences> _logger;

        public UpdateUserPreferences(IIdamsSyncService idamsSyncService, ILogger<UpdateUserPreferences> logger)
        {
            _idamsSyncService = idamsSyncService;
            _logger = logger;
        }

        public async Task UdateUserPreferencesJob([TimerTrigger("* */15 * * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            _logger.LogInformation("UpdateUsersFromIdams job started");
            try
            {
                var timer = Stopwatch.StartNew();
                //await Task.CompletedTask;
                await _idamsSyncService.SyncUsers();
                timer.Stop();
                _logger.LogInformation(
                    $"UpdateUsersFromIdams job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (AggregateException exc)
            {
                _logger.LogError(exc, "Error running UpdateUsersFromIdams WebJob");
                exc.Handle(ex =>
                {
                    _logger.LogError(ex, "Inner exception running UpdateUsersFromIdams WebJob");
                    return false;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running UpdateUsersFromIdams WebJob");
                throw;
            }
        }
    }
}
