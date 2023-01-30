using MoreLinq;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Services
{
    public class ImportProviderService : IImportProvider
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
            var batches = providers.Batch(1000).Select(b => b.ToDataTable(p => p.Ukprn, p => p.Name));
            
            foreach (var batch in batches)
            {
                await ImportProviders(batch);
            }

            _logger.LogInformation("ImportProvidersJob - Finished");
        }

        private Task ImportProviders(DataTable providersDataTable)
        {
            return _providerRepository.ImportProviders(providersDataTable);
        }
    }
}
