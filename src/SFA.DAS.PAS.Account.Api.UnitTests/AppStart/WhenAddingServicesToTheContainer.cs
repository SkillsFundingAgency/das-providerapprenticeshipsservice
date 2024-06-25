using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Authentication;
using SFA.DAS.PAS.Account.Api.Authorization;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Api.ServiceRegistrations;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement;
using SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
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
    [TestCase(typeof(IRequestHandler<SendNotificationCommand>))]
    [TestCase(typeof(IRequestHandler<GetUserNotificationSettingsQuery, GetUserNotificationSettingsResponse>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var hostEnvironment = new Mock<IWebHostEnvironment>();
        hostEnvironment.Setup(x => x.EnvironmentName).Returns("Test");
        var serviceCollection = new ServiceCollection();

        var configuration = GenerateConfiguration();

        serviceCollection.AddSingleton(hostEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IConfiguration>());

        var isDevOrLocal = configuration.IsDevOrLocal();

        serviceCollection
            .AddApiAuthentication(configuration)
            .AddApiAuthorization(isDevOrLocal);

        serviceCollection.AddOptions();
        serviceCollection.AddConfigurationOptions(configuration);
        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAccountUsersHandler>());
        serviceCollection.AddOrchestrators();
        serviceCollection.AddDataRepositories();
        serviceCollection.AddApiValidators();
        serviceCollection.AddApplicationServices();
        serviceCollection.AddLogging();
        serviceCollection.AddTransient<IBackgroundNotificationService, BackgroundNotificationService>();
        serviceCollection.AddTransient<IMessageSession, StubMessageSession>();
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.That(type, Is.Not.Null);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("ProviderAccountsApiConfiguration:ConnectionString", "test"),
                new("EnvironmentName", "LOCAL"),
                new("ProviderApprenticeshipsServiceConfiguration:DatabaseConnectionString", "test"),
                new("NotificationApi:ApiBaseUrl", "https://test"),
                new("NotificationApi:ClientToken", "token")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}

public class StubMessageSession : IMessageSession
{
    public Task Send(object message, SendOptions options) => throw new NotImplementedException();
    public Task Send<T>(Action<T> messageConstructor, SendOptions options) => throw new NotImplementedException();
    public Task Publish(object message, PublishOptions options) => throw new NotImplementedException();
    public Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions) => throw new NotImplementedException();
    public Task Subscribe(Type eventType, SubscribeOptions options) => throw new NotImplementedException();
    public Task Unsubscribe(Type eventType, UnsubscribeOptions options) => throw new NotImplementedException();
}