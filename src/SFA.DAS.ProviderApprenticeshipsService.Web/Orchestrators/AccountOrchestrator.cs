using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class AccountOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AccountOrchestrator(IMediator mediator, ILog logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _logger = logger;
        }

        public async Task<AccountHomeViewModel> GetProvider(int providerId)
        {
            try
            {
                _logger.Info($"Getting provider {providerId}");

                var providers = await _mediator.SendAsync(new GetProviderQueryRequest { UKPRN = providerId });

                var provider = providers.ProvidersView.Providers.First();

                return new AccountHomeViewModel {AccountStatus = AccountStatus.Active, ProviderName = provider.ProviderName, ProviderId = providerId};
            }
            catch (EntityNotFoundException)
            {
                _logger.Warn($"Provider {providerId} details not found in provider information service");

                return new AccountHomeViewModel {AccountStatus = AccountStatus.NoAgreement};
            }
        }
    }
}
