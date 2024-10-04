using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser;

public class GetUserHandler(IUserRepository userRepository) : IRequestHandler<GetUserQuery, GetUserResponse>
{
    public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserRef))
        {
            throw new InvalidRequestException(
                new List<ValidationFailure>{ new ValidationFailure("UserRef", "UserRef is null or empty") });
        }
        var user = await userRepository.GetUser(request.UserRef);

        if (user == null) 
        {
            return new GetUserResponse { };
        }

        return new GetUserResponse
        {
            Name = user.DisplayName,
            EmailAddress = user.Email
        };
    }
}