using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob
{
    public class IdamsSyncService : IIdamsSyncService
    {
        private readonly IIdamsEmailServiceWrapper _idamsEmailServiceWrapper;
        private readonly IUserRepository _userRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly ILog _logger;

        public IdamsSyncService(IIdamsEmailServiceWrapper idamsEmailServiceWrapper, IUserRepository userRepository, IProviderRepository providerRepository, ILog logger)
        {
            _idamsEmailServiceWrapper = idamsEmailServiceWrapper;
            _userRepository = userRepository;
            _providerRepository = providerRepository;
            _logger = logger;
        }

        public async Task SyncUsers()
        {
            List<IdamsUser> idamsUsers;

            var provider = await _providerRepository.GetNextProviderForIdamsUpdate();

            if (provider == null)
            {
                _logger.Info($"SyncUsers - No Provider Found");
                return;
            }

            _logger.Info($"SyncUsers For Provider {provider.Ukprn} has started");

            try
            {
                _logger.Info($"Retrieving DAS Users and Super Users for Provider {provider.Ukprn}");
                idamsUsers = await GetIdamsUsers(provider.Ukprn);

                _logger.Info($"Synchronise Users with IDAMS for Provider {provider.Ukprn}");
                await _userRepository.SyncIdamsUsers(provider.Ukprn, idamsUsers);

                await _providerRepository.MarkProviderIdamsUpdated(provider.Ukprn);
            }
            catch (HttpRequestException httpRequestEx)
            {
               
                //Can't get http status code from HttpRequestException is in the message hence
                if (!httpRequestEx.Message.Contains(((int)HttpStatusCode.NotFound).ToString()))
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
            _logger.Warn(ex, errorMessage);
            await _providerRepository.MarkProviderIdamsUpdated(provider.Ukprn);
        }

        private async Task<List<IdamsUser>> GetIdamsUsers(long providerId)
        {
            Task<List<string>> idamsUsersTask;
            Task<List<string>> idamsSuperUsersTask;

            idamsUsersTask = _idamsEmailServiceWrapper.GetEmailsAsync(providerId);
            idamsSuperUsersTask = _idamsEmailServiceWrapper.GetSuperUserEmailsAsync(providerId);

            await Task.WhenAll(idamsUsersTask, idamsSuperUsersTask);

            var idamsUsers = (await idamsUsersTask).Distinct();
            var idamsSuperUsers = (await idamsSuperUsersTask).Distinct();

            var idamsNormalUsers = idamsUsers.Where(u => !idamsSuperUsers.Any(su => su.Equals(u, StringComparison.InvariantCultureIgnoreCase)));

            return idamsNormalUsers.Select(u => new IdamsUser { Email = u, UserType = UserType.NormalUser }).Concat(idamsSuperUsers.Select(su => new IdamsUser { Email = su, UserType = UserType.SuperUser })).ToList();
        }
    }
}
