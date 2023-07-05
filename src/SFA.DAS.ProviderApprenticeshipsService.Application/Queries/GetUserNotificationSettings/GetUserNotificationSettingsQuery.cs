using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsQuery : IRequest<GetUserNotificationSettingsResponse>
{
    public string UserRef { get; set; }
}