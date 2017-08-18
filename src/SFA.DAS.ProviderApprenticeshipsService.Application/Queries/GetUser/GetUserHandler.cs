using System.Collections.Generic;
using System.Threading.Tasks;

using FluentValidation.Results;
using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser
{
    public class GetUserHandler : IAsyncRequestHandler<GetUserQuery, GetUserResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserResponse> Handle(GetUserQuery request)
        {
            if (string.IsNullOrEmpty(request.UserRef))
            {
                throw new InvalidRequestException(
                    new List<ValidationFailure>{ new ValidationFailure("UserRef", "UserRef is null or empty") });
            }
            var user = await _userRepository.GetUser(request.UserRef);

            return new GetUserResponse
                       {
                           Name = user.DisplayName,
                           EmailAddress = user.Email
                       };
        }
    }
}