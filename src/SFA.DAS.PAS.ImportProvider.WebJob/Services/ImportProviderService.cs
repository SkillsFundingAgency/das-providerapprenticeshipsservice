using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using Microsoft.Extensions.Logging;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Services
{
    public class ImportProviderService : IImportProviderService
    {
        private IProviderCommitmentsApi _providerApiClient;
        private IProviderRepository _providerRepository;
        private ILogger<ImportProviderService> _logger;

        public ImportProviderService(IProviderCommitmentsApi providerApiClient, IProviderRepository providerRepository, ILogger<ImportProviderService> logger)
        {
            _providerApiClient = providerApiClient;
            _providerRepository = providerRepository;
            _logger = logger;
        }

        public async Task Import()
        {
            _logger.LogInformation("Import Provider - Started");

            var providers = (await _providerApiClient.GetProviders()).Providers;
            var batches = providers.Chunk(1000);

            foreach (var batch in batches)
            {
                await ImportProviders(batch);
            }

            _logger.LogInformation("ImportProvidersJob - Finished");
        }

        private Task ImportProviders(ProviderResponse[] providers)
        {
            return _providerRepository.ImportProviders(providers);
        }
    }
}
