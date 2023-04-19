using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Services;

public class ImportProviderService : IImportProviderService
{
    private readonly ICommitmentsV2ApiClient _commitmentsV2ApiClient;
    private readonly IProviderRepository _providerRepository;
    private readonly ILogger<ImportProviderService> _logger;

    public ImportProviderService(ICommitmentsV2ApiClient commitmentsV2ApiClient, IProviderRepository providerRepository, ILogger<ImportProviderService> logger)
    {
        _commitmentsV2ApiClient = commitmentsV2ApiClient;
        _providerRepository = providerRepository;
        _logger = logger;
    }

    public async Task Import()
    {
        _logger.LogInformation("Import Provider - Started");

        var providers = (await _commitmentsV2ApiClient.GetProviders()).Providers;
        var batches = providers.Chunk(1000);

        foreach (var batch in batches)
        {
            await ImportProviders(batch);
        }

        _logger.LogInformation("ImportProvidersJob - Finished");
    }

    private Task ImportProviders(Provider[] providers)
    {
        return _providerRepository.ImportProviders(providers);
    }
}