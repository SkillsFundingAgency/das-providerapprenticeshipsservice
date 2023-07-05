using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class ValidationServiceRegistration
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<DeleteRegisteredUserCommand>, DeleteRegisteredUserCommandValidator>();
        services.AddScoped<IValidator<SendNotificationCommand>, SendNotificationCommandValidator>();
        services.AddScoped<IValidator<UpdateUserNotificationSettingsCommand>, UpdateUserNotificationSettingsValidator>();
        services.AddScoped<IValidator<UpsertRegisteredUserCommand>, UpsertRegisteredUserCommandValidator>();

        return services;
    }
}