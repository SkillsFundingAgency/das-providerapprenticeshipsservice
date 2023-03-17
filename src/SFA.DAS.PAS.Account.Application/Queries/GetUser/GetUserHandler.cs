using FluentValidation.Results;
using MediatR;
using SFA.DAS.PAS.Account.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUser
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, GetUserResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
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
                EmailAddress = user.Email,
                IsSuperUser = (user.UserType.Equals(UserType.SuperUser)) ? true : false
            };
        }
    }
}