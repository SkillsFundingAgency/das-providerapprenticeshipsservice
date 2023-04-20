using MediatR;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.PAS.Account.Application.Commands.SendNotification;

public class SendNotificationCommand : IRequest<Unit>
{
    public Email Email { get; }

    public SendNotificationCommand(Email email)
    {
        Email = email;
    }
}