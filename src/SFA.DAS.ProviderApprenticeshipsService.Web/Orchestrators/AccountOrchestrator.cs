using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class AccountOrchestrator
    {
        private readonly IMediator _mediator;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public AccountOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<AccountHomeViewModel> GetProvider(int providerId)
        {
            try
            {
                var providers = await _mediator.SendAsync(new GetProviderQueryRequest { UKPRN = providerId });

                var provider = providers.ProvidersView.Providers.First();

                return new AccountHomeViewModel {AccountStatus = AccountStatus.Active, ProviderName = provider.ProviderName, ProviderId = providerId};
            }
            catch (EntityNotFoundException)
            {
                Logger.Warn($"Provider {providerId} details not found in provider information service");

                return new AccountHomeViewModel {AccountStatus = AccountStatus.NoAgreement};
            }
        }
    }
}
