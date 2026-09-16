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

namespace SFA.DAS.PAS.Jobs.Services;

public class UserSyncService(
    IUserRepository userRepository,
    IProviderRepository providerRepository,
    ILogger<UserSyncService> logger,
    IApiHelper apiHelper,
    DfEOidcConfiguration dfEOidcConfiguration) : IUserSyncService
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
        logger.LogWarning(ex, "{ErrorMessage}", errorMessage);
        return providerRepository.MarkProviderIdamsUpdated(provider.Ukprn);
    }

    private async Task<List<IdamsUser>> GetIdamsUsers(long providerId)
    {
        var endpoint = $"{dfEOidcConfiguration.APIServiceUrl}/organisations/{providerId}/users";
        var response = await apiHelper.Get<DfeUser>(endpoint);

        if (response?.Users == null)
        {
            logger.LogInformation("{Endpoint} - None found", endpoint);
            return [];
        }

        logger.LogInformation("{Endpoint} - Found {Count}", endpoint, response.Users.Count);

        return response.Users
            .Where(u => u.UserStatus == 1)
            .GroupBy(u => u.Email, StringComparer.OrdinalIgnoreCase)
            .Select(g => new IdamsUser { Email = g.Key, UserType = UserType.NormalUser })
            .ToList();
    }
}
