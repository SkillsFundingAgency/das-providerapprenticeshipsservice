using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.IdamsUser;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services
{
    public class IdamsSyncService : IIdamsSyncService
    {
        private readonly IIdamsEmailServiceWrapper _idamsEmailServiceWrapper;
        private readonly IUserRepository _userRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly ILogger<IdamsSyncService> _logger;
        private readonly ProviderNotificationConfiguration _configuration;

        public IdamsSyncService(
            IIdamsEmailServiceWrapper idamsEmailServiceWrapper,
            IUserRepository userRepository,
            IProviderRepository providerRepository,
            ILogger<IdamsSyncService> logger,
            ProviderApprenticeshipsServiceConfiguration configuration)
        {
            _idamsEmailServiceWrapper = idamsEmailServiceWrapper;
            _userRepository = userRepository;
            _providerRepository = providerRepository;
            _logger = logger;
            _configuration = configuration.CommitmentNotification;
        }

        public async Task SyncUsers()
        {
            List<IdamsUser> idamsUsers;

            var provider = await _providerRepository.GetNextProviderForIdamsUpdate();

            if (provider == null)
            {
                _logger.LogInformation($"SyncUsers - No Provider Found");
                return;
            }

            _logger.LogInformation($"SyncUsers For Provider {provider.Ukprn} has started");

            try
            {
                _logger.LogInformation($"Retrieving DAS Users and Super Users for Provider {provider.Ukprn}");
                idamsUsers = await GetIdamsUsers(provider.Ukprn);

                _logger.LogInformation($"Synchronise Users with IDAMS for Provider {provider.Ukprn}");
                await _userRepository.SyncIdamsUsers(provider.Ukprn, idamsUsers);

                await _providerRepository.MarkProviderIdamsUpdated(provider.Ukprn);
            }
            catch (CustomHttpRequestException httpRequestEx)
            {

                //Can't get http status code from HttpRequestException is in the message hence
                if (httpRequestEx.StatusCode != HttpStatusCode.NotFound)
                {
                    string message = $"An error occurred retrieving users from Provider {provider.Ukprn}";
                    await LogAndUpdateProviderState(httpRequestEx, provider, message);
                    throw;
                }

                string httpNotFoundMessage = $"There are no super users (or any users) for Provider {provider.Ukprn}";
                await LogAndUpdateProviderState(httpRequestEx, provider, httpNotFoundMessage);

            }
            catch (Exception ex)
            {
                string message = $"An error occurred retrieving users from Provider {provider.Ukprn}";
                await LogAndUpdateProviderState(ex, provider, message);
                throw;
            }

        }

        private async Task LogAndUpdateProviderState(Exception ex, Provider provider, string errorMessage)
        {
            _logger.LogWarning(ex, errorMessage);
            await _providerRepository.MarkProviderIdamsUpdated(provider.Ukprn);
        }

        private async Task<List<IdamsUser>> GetIdamsUsers(long providerId)
        {
            var idamsUsersTask = _idamsEmailServiceWrapper.GetEmailsAsync(providerId, _configuration.DasUserRoleId);
            var idamsSuperUsersTask = _idamsEmailServiceWrapper.GetEmailsAsync(providerId, _configuration.SuperUserRoleId);

            await Task.WhenAll(idamsUsersTask, idamsSuperUsersTask);

            var idamsUsers = (await idamsUsersTask).Distinct();
            var idamsSuperUsers = (await idamsSuperUsersTask).Distinct();

            var idamsNormalUsers = idamsUsers.Where(u => !idamsSuperUsers.Any(su => su.Equals(u, StringComparison.InvariantCultureIgnoreCase)));

            return idamsNormalUsers.Select(u => new IdamsUser { Email = u, UserType = UserType.NormalUser }).Concat(idamsSuperUsers.Select(su => new IdamsUser { Email = su, UserType = UserType.SuperUser })).ToList();
        }
    }
}
