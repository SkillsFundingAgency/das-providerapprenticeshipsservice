using SFA.DAS.ProviderApprenticeshipsService.Application.Services.UserIdentityService;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

public interface IAuthenticationOrchestrator
{
    Task<bool> SaveIdentityAttributes(string userId, string ukprn, string displayName, string email);
}

public class AuthenticationOrchestrator : IAuthenticationOrchestrator
{
    private readonly IProviderCommitmentsLogger _logger;
    private readonly IUserIdentityService _userIdentityService;

    public AuthenticationOrchestrator(
        IProviderCommitmentsLogger logger,
        IUserIdentityService userIdentityService)
    {
        _logger = logger;
        _userIdentityService = userIdentityService;
    }

    public async Task<bool> SaveIdentityAttributes(string userId, string ukprn, string displayName, string email)
    {
        long parsedUkprn;

        if (!long.TryParse(ukprn, out parsedUkprn))
        {
            _logger.Info($"Unable to parse Ukprn \"{ukprn}\" from claims for user \"{userId}\"");
            return false;
        }

        _logger.Info($"Updating \"{userId}\" attributes - ukprn:\"{parsedUkprn}\", displayname:\"{displayName}\", email:\"{email}\"");

        await _userIdentityService.UpsertUserIdentityAttributes(userId, parsedUkprn, displayName, email);

        return true;
    }
}