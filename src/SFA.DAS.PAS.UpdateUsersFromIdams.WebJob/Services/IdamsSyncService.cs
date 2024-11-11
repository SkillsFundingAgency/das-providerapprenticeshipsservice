using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.DfESignInUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.IdamsUser;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;

public class IdamsSyncService(
    IUserRepository userRepository,
    IProviderRepository providerRepository,
    ILogger<IdamsSyncService> logger,
    IApiHelper apiHelper,
    DfEOidcConfiguration dfEOidcConfiguration)
    : IIdamsSyncService
{
    public async Task SyncUsers()
    {
        var provider = await providerRepository.GetNextProviderForIdamsUpdate();

        if (provider == null)
        {
            logger.LogInformation("SyncUsers - No Provider Found");
            return;
        }

        logger.LogInformation("SyncUsers For Provider {Ukprn} has started", provider.Ukprn);

        try
        {
            logger.LogInformation("Retrieving DAS Users for Provider {Ukprn}", provider.Ukprn);
            var idamsUsers = await GetIdamsUsers(provider.Ukprn);

            logger.LogInformation("Synchronise Users with IDAMS for Provider {Ukprn}", provider.Ukprn);
            await userRepository.SyncIdamsUsers(provider.Ukprn, idamsUsers);

            await providerRepository.MarkProviderIdamsUpdated(provider.Ukprn);
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

            var httpNotFoundMessage = $"There are no users for Provider {provider.Ukprn}";
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
        logger.LogWarning(ex, errorMessage);
        return providerRepository.MarkProviderIdamsUpdated(provider.Ukprn);
    }

    private async Task<IEnumerable<IdamsUser>> GetIdamsUsers(long providerId)
    {
        var response = await apiHelper.Get<DfeUser>($"{dfEOidcConfiguration.APIServiceUrl}/organisations/{providerId}/users");
        
        if (response == null)
        {
            logger.LogInformation("{APIServiceUrl}/organisations/{ProviderId}/users - None found", dfEOidcConfiguration.APIServiceUrl, providerId);
            return [];
        }

        logger.LogInformation("{APIServiceUrl}/organisations/{ProviderId}/users - Found {UsersCount}", dfEOidcConfiguration.APIServiceUrl, providerId, response.Users.Count);

        return response.Users
            .Where(c => c.UserStatus == 1)
            .Distinct()
            .Select(x => new IdamsUser
            {
                Email = x.Email,
            });
    }
}