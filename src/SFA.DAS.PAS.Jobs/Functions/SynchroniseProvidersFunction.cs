using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Jobs.Services;

namespace SFA.DAS.PAS.Jobs.Functions;

public class SynchroniseProvidersFunction(IImportProviderService importProviderService, ILogger<SynchroniseProvidersFunction> logger)
{
    [Function(nameof(SynchroniseProvidersFunction))]
    public async Task Run([TimerTrigger("%SynchroniseProvidersFunctionSchedule%", RunOnStartup = false)] TimerInfo timerInfo)
    {
        logger.LogInformation("SynchroniseProvidersFunction started");

        await importProviderService.ImportProviders();

        logger.LogInformation("SynchroniseProvidersFunction completed");
    }
}
