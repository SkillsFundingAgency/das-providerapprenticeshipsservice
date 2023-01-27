using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;

namespace SFA.DAS.PAS.Account.Api;

public static class AddFluentValidationExtensions
{
    public static void AddFluentValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<SendNotificationCommand>, SendNotificationCommandValidator>();
    }
}