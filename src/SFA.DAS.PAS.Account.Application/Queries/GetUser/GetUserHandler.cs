using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUser
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, GetUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<IUserRepository> _logger;

        public GetUserHandler(IUserRepository userRepository, ILogger<IUserRepository> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserRef))
            {
                throw new InvalidRequestException(
                    new List<ValidationFailure>{ new ValidationFailure("UserRef", "UserRef is null or empty") });
            }
            var user = await _userRepository.GetUser(request.UserRef);

            if (user == null) 
            {
                _logger.LogInformation($"Unable to get user settings with ref {request.UserRef}");
                return new GetUserResponse { };
            }

            return new GetUserResponse
            {
                UserRef = user.UserRef,
                Name = user.DisplayName,
                EmailAddress = user.Email,
                IsSuperUser = (user.UserType.Equals(UserType.SuperUser)) ? true : false
            };
        }
    }
}