using MediatR;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsQuery : IRequest<GetUserNotificationSettingsResponse>
{
    public string? UserRef { get; set; }
}