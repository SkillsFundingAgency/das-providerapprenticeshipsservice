using MoreLinq;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Providers.Api.Client;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Importer
{
    public class ImportProviderService : IImportProvider
    {
        private IProviderApiClient _providerApiClient;
        private IImportProviderRepository _importProviderRepository;
        private ILog _logger;

        public ImportProviderService(IProviderApiClient providerApiClient, IImportProviderRepository importProviderRepository, ILog logger)
        {
            _providerApiClient = providerApiClient;
            _importProviderRepository = importProviderRepository;
            _logger = logger;
        }

        public async Task Import()
        {
            _logger.Info("Import Provider - Started");

            var providers = await _providerApiClient.FindAllAsync();
            var batches = providers.Batch(1000).Select(b => b.ToDataTable(p => p.Ukprn, p => p.ProviderName));

            foreach (var batch in batches)
            {
                await ImportProviders(batch);
            }

            _logger.Info("ImportProvidersJob - Finished");
        }

        private Task ImportProviders(DataTable providersDataTable)
        {
            return _importProviderRepository.ImportProviders(providersDataTable);
        }
    }
}
