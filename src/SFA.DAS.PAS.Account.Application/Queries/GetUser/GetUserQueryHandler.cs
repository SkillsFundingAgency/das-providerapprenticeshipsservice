using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUser;

public class GetUserQueryHandler(IUserRepository userRepository, ILogger<GetUserQueryHandler> logger)
    : IRequestHandler<GetUserQuery, GetUserQueryResponse>
{
    public async Task<GetUserQueryResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserRef))
        {
            throw new InvalidRequestException(new List<ValidationFailure>{ new("UserRef", "UserRef is null or empty") });
        }
        var user = await userRepository.GetUser(request.UserRef);

        if (user == null) 
        {
            logger.LogInformation("Unable to get user settings with ref {UserRef}", request.UserRef);
            return new GetUserQueryResponse();
        }

        return new GetUserQueryResponse
        {
            UserRef = user.UserRef,
            Name = user.DisplayName,
            EmailAddress = user.Email
        };
    }
}