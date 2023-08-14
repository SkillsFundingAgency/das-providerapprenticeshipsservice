using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.UserIdentityService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;

public class UpsertRegisteredUserCommandHandler : IRequestHandler<UpsertRegisteredUserCommand>
{
    private readonly IValidator<UpsertRegisteredUserCommand> _validator;
    private readonly IUserIdentityService _userIdentityService;

    public UpsertRegisteredUserCommandHandler(
        IValidator<UpsertRegisteredUserCommand> validator,
        IUserIdentityService userIdentityService)
    {
        _validator = validator;
        _userIdentityService = userIdentityService;
    }

    public async Task Handle(UpsertRegisteredUserCommand message, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(message);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        await _userIdentityService.UpsertUserIdentityAttributes(message.UserRef, message.Ukprn, message.DisplayName, message.Email);
    }
}