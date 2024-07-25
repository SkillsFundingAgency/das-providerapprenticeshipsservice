using System.Diagnostics.CodeAnalysis;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class RoutingExtensions
{
    public static void AddRouting(this RoutingSettings routing)
    {
        routing.RouteToEndpoint(
            typeof(SendEmailCommand).Assembly,
            typeof(SendEmailCommand).Namespace,
            "SFA.DAS.Notifications.MessageHandlers"
        );
    }
}