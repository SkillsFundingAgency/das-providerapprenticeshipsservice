using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.PAS.Jobs.ApiClients;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.PAS.Jobs.Services;

public class ImportProviderService(
    IRoatpApiClient roatpApiClient,
    IProviderRepository providerRepository,
    ILogger<ImportProviderService> logger) : IImportProviderService
{
    private const int BatchSize = 1000;

    public async Task ImportProviders()
    {
        logger.LogInformation("Import Providers Service - Started");

        ApiModels.GetAllProvidersResponse providersResponse = await roatpApiClient.GetProviders();
        var providers = providersResponse?.Organisations;

        if (providers.Count == 0)
        {
            logger.LogInformation("ImportProvidersJob - No providers returned");
            return;
        }

        foreach (var batch in providers.Chunk(BatchSize))
        {
            var mappedProviders = batch
                .Select(provider => new Provider
                {
                    Ukprn = provider.Ukprn,
                    Name = provider.LegalName,
                })
                .ToArray();

            await providerRepository.ImportProviders(mappedProviders);
        }

        logger.LogInformation("Import Providers Service - Finished");
    }
}
