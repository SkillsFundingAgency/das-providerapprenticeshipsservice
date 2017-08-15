using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsValidator : AbstractValidator<UpdateUserNotificationSettingsCommand>
    {
        public UpdateUserNotificationSettingsValidator()
        {
            RuleFor(m => m.UserRef).NotNull().NotEmpty();
        }

    }
}