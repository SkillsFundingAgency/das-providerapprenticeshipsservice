using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Newtonsoft.Json.Converters;
using SFA.DAS.PAS.Account.Api.Formatters;

namespace SFA.DAS.PAS.Account.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configure Web API to use only bearer token authentication.
            var jsonChunkFormatter = new ChunkJsonMediaTypeFormatter() { SerializerSettings = config.Formatters.JsonFormatter.SerializerSettings };
            config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.Insert(0, jsonChunkFormatter);

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            config.MapHttpAttributeRoutes();

            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());
        }
    }
}


