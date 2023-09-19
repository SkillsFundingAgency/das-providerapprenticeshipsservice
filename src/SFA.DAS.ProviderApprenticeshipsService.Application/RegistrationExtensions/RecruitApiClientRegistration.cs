using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class RecruitApiClientRegistration
    {
        public static IServiceCollection AddRecruitApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RecruitApiConfiguration>(configuration.GetSection(nameof(RecruitApiConfiguration)));
            services.AddHttpClient<IRecruitApiClient, RecruitApiClient>();
            services.AddTransient<ITrainingProviderService, TrainingProviderService>();
            return services;
        }
    }
}
