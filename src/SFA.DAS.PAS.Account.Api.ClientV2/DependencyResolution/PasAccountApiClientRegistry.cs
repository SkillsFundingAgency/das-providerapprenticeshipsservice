using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;
using StructureMap;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.PAS.Account.Api.ClientV2.DependencyResolution
{
    public class PasAccountApiClientRegistry : Registry
    {
        public PasAccountApiClientRegistry(Expression<Func<IContext, PasAccountApiConfiguration>> getApiConfig)
        {
            For<PasAccountApiConfiguration>().Use(getApiConfig);
            For<IPasAccountApiClient>().Use(ctx => CreateClient(ctx)).Singleton();
        }

        private IPasAccountApiClient CreateClient(IContext ctx)
        {
            var config = ctx.GetInstance<PasAccountApiConfiguration>();
            var loggerFactory = ctx.GetInstance<ILoggerFactory>();

            var factory = new PasAccountApiClientFactory(config, loggerFactory);
            return factory.CreateClient();
        }
    }
}