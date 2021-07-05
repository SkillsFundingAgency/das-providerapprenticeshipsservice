using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public interface IIdamsEmailServiceWrapper
    {
        Task<List<string>> GetEmailsAsync(long ukprn);
        Task<List<string>> GetSuperUserEmailsAsync(long providerId);
    }

    public class IdamsEmailServiceWrapper : IIdamsEmailServiceWrapper
    {
        private readonly ILog _logger;
        private readonly ProviderNotificationConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClientWrapper;

        public IdamsEmailServiceWrapper(
            ILog logger,
            ProviderApprenticeshipsServiceConfiguration configuration,
            IHttpClientWrapper httpClientWrapper)
        {
            _logger = logger;
            _configuration = configuration.CommitmentNotification;
            _httpClientWrapper = httpClientWrapper;
        }

        public virtual async Task<List<string>> GetEmailsAsync(long providerId)
        {
            var url = string.Format(_configuration.IdamsListUsersUrl, _configuration.DasUserRoleId, providerId);
            _logger.Info($"Getting 'DAS' emails for provider {providerId}");
            var result = await GetString(url, _configuration.ClientToken);
            return ParseIdamsResult(result, providerId);
        }

        public virtual async Task<List<string>> GetSuperUserEmailsAsync(long providerId)
        {
            var url = string.Format(_configuration.IdamsListUsersUrl, _configuration.SuperUserRoleId, providerId);
            _logger.Info($"Getting 'super user' emails for provider {providerId}");
            var result = GetString(url, _configuration.ClientToken);
            return ParseIdamsResult(await result, providerId);
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
            catch (Exception exception)
            {
                var resultDescription = string.IsNullOrWhiteSpace(jsonResult) ? "empty string" : $"\"{jsonResult}\"";
                throw new ArgumentException($"Not possible to parse {resultDescription} to {typeof(UserResponse)} for provider: {providerId}", exception);
            }
        }

        private async Task<string> GetString(string url, string accessToken)
        {
            return await _httpClientWrapper.GetStringFromResponseAsync(url);
        }
    }
}
