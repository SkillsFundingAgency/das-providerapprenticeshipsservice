using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserQueryResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserQueryHandler> _logger;

    public GetUserQueryHandler(IUserRepository userRepository, ILogger<GetUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetUserQueryResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserRef))
        {
            throw new InvalidRequestException(new List<ValidationFailure>{ new("UserRef", "UserRef is null or empty") });
        }
        var user = await _userRepository.GetUser(request.UserRef);

        if (user == null) 
        {
            _logger.LogInformation("Unable to get user settings with ref {UserRef}", request.UserRef);
            return new GetUserQueryResponse { };
        }

        return new GetUserQueryResponse
        {
            UserRef = user.UserRef,
            Name = user.DisplayName,
            EmailAddress = user.Email,
            IsSuperUser = user.UserType.Equals(UserType.SuperUser)
        };
    }
}