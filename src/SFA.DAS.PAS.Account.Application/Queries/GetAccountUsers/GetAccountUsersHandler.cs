using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;

public class GetAccountUsersHandler : IRequestHandler<GetAccountUsersQuery, GetAccountUsersResponse>
{
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAccountUsersHandler> _logger;

    public GetAccountUsersHandler(
        IUserSettingsRepository userSettingsRepository, 
        IUserRepository userRepository,
        ILogger<GetAccountUsersHandler> logger)
    {
        _userSettingsRepository = userSettingsRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetAccountUsersResponse> Handle(GetAccountUsersQuery request, CancellationToken cancellationToken)
    {
        if(request.Ukprn < 1)
            throw new ValidationException("Ukprn must be more than 0 when getting account users.");

        var response = new GetAccountUsersResponse();

        _logger.LogInformation("Getting users from repository for {Ukprn}", request.Ukprn);

        var providerUsers = (await _userRepository.GetUsers(request.Ukprn)).ToList();
        
        if (!providerUsers.Any()) 
        {
            return response;
        }

        _logger.LogInformation("Retrieved {ProviderUsersCount} users from repository for {Ukprn}", providerUsers.Count, request.Ukprn);

        foreach (var user in providerUsers)
        {
            var settings = (await _userSettingsRepository.GetUserSetting(user.UserRef)).ToList();
          
            if (settings.Any())
            {
                response.Add(user, settings.FirstOrDefault());
            }
        }

        return response;
    }
}