using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.DfESignInUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.IdamsUser;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;

public class IdamsSyncService : IIdamsSyncService
{
    private readonly IUserRepository _userRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly ILogger<IdamsSyncService> _logger;
    private readonly IApiHelper _apiHelper;
    private readonly DfEOidcConfiguration _dfEOidcConfiguration;

    public IdamsSyncService(
        IUserRepository userRepository,
        IProviderRepository providerRepository,
        ILogger<IdamsSyncService> logger,
        IApiHelper apiHelper,
        DfEOidcConfiguration dfEOidcConfiguration)
    {
        _userRepository = userRepository;
        _providerRepository = providerRepository;
        _logger = logger;
        _apiHelper = apiHelper;
        _dfEOidcConfiguration = dfEOidcConfiguration;
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
                var message = $"An error occurred retrieving users from Provider {provider.Ukprn}";
                await LogAndUpdateProviderState(httpRequestEx, provider, message);
                throw;
            }

            var httpNotFoundMessage = $"There are no super users (or any users) for Provider {provider.Ukprn}";
            await LogAndUpdateProviderState(httpRequestEx, provider, httpNotFoundMessage);

        }
        catch (Exception ex)
        {
            var message = $"An error occurred retrieving users from Provider {provider.Ukprn}";
            await LogAndUpdateProviderState(ex, provider, message);
            throw;
        }

    }

    private Task LogAndUpdateProviderState(Exception ex, Provider provider, string errorMessage)
    {
        _logger.LogWarning(ex, errorMessage);
        return _providerRepository.MarkProviderIdamsUpdated(provider.Ukprn);
    }

    private async Task<List<IdamsUser>> GetIdamsUsers(long providerId)
    {
        var response = await _apiHelper.Get<DfeUser>($"{_dfEOidcConfiguration.APIServiceUrl}/organisations/{providerId}/users");
        if (response == null)
        {
            _logger.LogInformation($"{_dfEOidcConfiguration.APIServiceUrl}/organisations/{providerId}/users - None found");
            return new List<IdamsUser>();
        }
        
        _logger.LogInformation($"{_dfEOidcConfiguration.APIServiceUrl}/organisations/{providerId}/users - Found {response.Users.Count}");

        var idamsUsers = response.Users.Where(c=>c.UserStatus ==1 ).Distinct();
        var idamsSuperUsers = new List<IdamsUser>();

        var idamsNormalUsers = idamsUsers.Where(u => !idamsSuperUsers.Any(su => su.Email.Equals(u.Email, StringComparison.InvariantCultureIgnoreCase)));

        return idamsNormalUsers.Select(u => new IdamsUser { Email = u.Email, UserType = UserType.NormalUser }).Concat(idamsSuperUsers.Select(su => new IdamsUser { Email = su.Email, UserType = UserType.SuperUser })).ToList();
    }
}

