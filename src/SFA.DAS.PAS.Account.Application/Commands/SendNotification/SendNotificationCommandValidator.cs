using FluentValidation;

namespace SFA.DAS.PAS.Account.Application.Commands.SendNotification
{
    public sealed class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
    {
        public SendNotificationCommandValidator()
        {
            RuleFor(x => x.Email).NotNull();

            When(x => x.Email != null, () =>
            {
                RuleFor(x => x.Email.RecipientsAddress).NotEmpty();
                RuleFor(x => x.Email.ReplyToAddress).NotEmpty();
                RuleFor(x => x.Email.Subject).NotEmpty();
                RuleFor(x => x.Email.TemplateId).NotEmpty();
            });
        }
    }
}