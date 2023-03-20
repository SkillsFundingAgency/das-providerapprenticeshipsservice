using FluentAssertions.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Api.ServiceRegistrations;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement;
using SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.PAS.Account.Api.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(IAccountOrchestrator))]
    [TestCase(typeof(IEmailOrchestrator))]
    [TestCase(typeof(IUserOrchestrator))]
    [TestCase(typeof(IBackgroundNotificationService))]
    [TestCase(typeof(IProviderCommitmentsLogger))]
    [TestCase(typeof(ICurrentDateTime))]
    [TestCase(typeof(IUserSettingsRepository))]
    [TestCase(typeof(IProviderAgreementStatusRepository))]
    [TestCase(typeof(IUserRepository))]
    [TestCase(typeof(IPasAccountApiConfiguration))]
    [TestCase(typeof(IRequestHandler<GetAccountUsersQuery, GetAccountUsersResponse>))]
    [TestCase(typeof(IRequestHandler<GetProviderAgreementQueryRequest, GetProviderAgreementQueryResponse>))]
    [TestCase(typeof(IRequestHandler<SendNotificationCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<GetUserNotificationSettingsQuery, GetUserNotificationSettingsResponse>))]
    [TestCase(typeof(NotificationsApiClientConfiguration))]
    [TestCase(typeof(INotificationsApi))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var hostEnvironment = new Mock<IWebHostEnvironment>();
        hostEnvironment.Setup(x => x.EnvironmentName).Returns("Test");
        var serviceCollection = new ServiceCollection();

        var configuration = GenerateConfiguration();

        serviceCollection.AddSingleton(hostEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
        serviceCollection.AddOptions();
        serviceCollection.AddConfigurationOptions(configuration);
        serviceCollection.AddMediatRHandlers();
        serviceCollection.AddOrchestrators();
        serviceCollection.AddDataRepositories();
        serviceCollection.AddFluentValidation();
        serviceCollection.AddLogging();
        serviceCollection.AddApplicationServices();
        serviceCollection.AddNotifications(configuration);

        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new ("ProviderAccountsApiConfiguration:ConnectionString", "test"),
                new ("EnvironmentName", "test"),
                new ("ProviderApprenticeshipsServiceConfiguration:DatabaseConnectionString", "test"),
                new ("NotificationsApi:ApiBaseUrl", "https://test")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> {provider});
    }
}