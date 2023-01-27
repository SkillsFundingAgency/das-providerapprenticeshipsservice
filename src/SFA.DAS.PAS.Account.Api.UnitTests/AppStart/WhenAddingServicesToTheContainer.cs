using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.PAS.Account.Api.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(IAccountOrchestrator))]
    [TestCase(typeof(IEmailOrchestrator))]
    [TestCase(typeof(IUserOrchestrator))]
    [TestCase(typeof(IRequestHandler<GetAccountUsersQuery, GetAccountUsersResponse>))]
    [TestCase(typeof(IRequestHandler<GetProviderAgreementQueryRequest, GetProviderAgreementQueryResponse>))]
    [TestCase(typeof(IRequestHandler<SendNotificationCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<GetUserNotificationSettingsQuery, GetUserNotificationSettingsResponse>))]
    [TestCase(typeof(ProviderApprenticeshipsServiceConfiguration))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var hostEnvironment = new Mock<IWebHostEnvironment>();
        var serviceCollection = new ServiceCollection();

        var configuration = GenerateConfiguration();
        
        serviceCollection.AddSingleton(hostEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
        serviceCollection.AddConfiguration(configuration);
        serviceCollection.AddDistributedMemoryCache();
        serviceCollection.AddServiceRegistration();
        serviceCollection.AddFluentValidation();
        serviceCollection.AddLogging();
        
        // var providerAccountsApiConfiguration = configuration
        //     .GetSection(nameof(ProviderAccountsApiConfiguration))
        //     .Get<ProviderAccountsApiConfiguration>();
        // serviceCollection.AddDatabaseRegistration(providerAccountsApiConfiguration, configuration["EnvironmentName"]);

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
                new KeyValuePair<string, string>("ProviderAccountsApiConfiguration:ConnectionString", "test"),
                new KeyValuePair<string, string>("EnvironmentName", "test")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> {provider});
    }
}