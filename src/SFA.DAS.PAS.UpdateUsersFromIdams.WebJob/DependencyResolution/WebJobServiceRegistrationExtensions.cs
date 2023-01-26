using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.DependencyResolution
{
    public static class WebJobServiceRegistrationExtensions
    {
        public static void AddWebJobServices(this IServiceCollection services)
        {
            services.AddTransient<IIdamsSyncService, IdamsSyncService>();
        }
    }

}