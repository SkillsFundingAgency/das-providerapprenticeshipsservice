using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsQuery : IAsyncRequest<GetUserNotificationSettingsResponse>
    {
        public string UserRef { get; set; }
    }
}