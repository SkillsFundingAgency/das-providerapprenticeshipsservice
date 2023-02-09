using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class ProviderRelationshipsApiClient
    {
        public static IServiceCollection AddProviderRelationshipsApi(this IServiceCollection services, IConfiguration configuration)
        {
            /*
             TO BE REMOVED IF CONFIRMED THAT NO USAGE IS PLANNED
            */
            services.Configure<ProviderRelationshipsApiConfiguration>(c => configuration.GetSection("ProviderRelationshipsApi").Bind(c));

            var useStub = GetUseStubProviderRelationshipsSetting();
            if (useStub)
            {
                services.AddTransient<IProviderRelationshipsApiClient, StubProviderRelationshipsApiClient>();
            }

            return services;
        }

        private static bool GetUseStubProviderRelationshipsSetting()
        {
            var value = ConfigurationManager.AppSettings["UseStubProviderRelationships"];

            if (value == null)
            {
                return false;
            }

            if (!bool.TryParse(value, out var result))
            {
                return false;
            }

            return result;
        }
    }
}
