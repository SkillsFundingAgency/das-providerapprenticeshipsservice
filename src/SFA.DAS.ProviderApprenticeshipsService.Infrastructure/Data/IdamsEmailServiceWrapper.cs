using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public interface IIdamsEmailServiceWrapper
    {
        Task<List<string>> GetEmailsAsync(long ukprn, string identities);
    }

    public class IdamsEmailServiceWrapper : IIdamsEmailServiceWrapper
    {
        private readonly ILogger<IdamsEmailServiceWrapper> _logger;
        private readonly ProviderNotificationConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClientWrapper;

        public IdamsEmailServiceWrapper(
            ILogger<IdamsEmailServiceWrapper> logger,
            ProviderApprenticeshipsServiceConfiguration configuration,
            IHttpClientWrapper httpClientWrapper)
        {
            _logger = logger;
            _configuration = configuration.CommitmentNotification;
            _httpClientWrapper = httpClientWrapper;
        }

        public virtual async Task<List<string>> GetEmailsAsync(long providerId, string roles)
        {
            _logger.LogInformation($"Getting emails for provider {providerId} for roles {roles}");

            var ids = roles.Split(',');
            var tasks = ids.Select(id => GetString(string.Format(_configuration.IdamsListUsersUrl, id, providerId)));
            var results = await Task.WhenAll(tasks);

            return results.SelectMany(result => ParseIdamsResult(result, providerId)).ToList();
        }

        private List<string> ParseIdamsResult(string jsonResult, long providerId)
        {
            try
            {
                var result = JObject.Parse(jsonResult).SelectToken("result");

                if (result.Type == JTokenType.Array)
                {
                    var items = result.ToObject<IEnumerable<UserResponse>>();
                    return items.SelectMany(m => m.Emails).ToList();
                }

                var item = result.ToObject<UserResponse>();
                return item?.Emails ?? new List<string>(0);
            }
            catch (JsonSerializationException)
            {
                // Idams query returned no results - { result : ["internal error"] }.
                return new List<string>();
            }
            catch (Exception exception)
            {
                var resultDescription = string.IsNullOrWhiteSpace(jsonResult) ? "empty string" : $"\"{jsonResult}\"";
                throw new ArgumentException($"Not possible to parse {resultDescription} to {typeof(UserResponse)} for provider: {providerId}", exception);
            }
        }

        private async Task<string> GetString(string url)
        {
            _logger.LogInformation($"Querying {url} for user details");
            return await _httpClientWrapper.GetStringAsync(url);
        }
    }
}
