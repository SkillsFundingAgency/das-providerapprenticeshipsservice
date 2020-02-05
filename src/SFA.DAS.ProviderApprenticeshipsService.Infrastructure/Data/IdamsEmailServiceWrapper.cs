using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        Task<List<string>> GetSuperUserEmailsAsync(long ukprn);
    }

    public class IdamsEmailServiceWrapper : IIdamsEmailServiceWrapper
    {

        private readonly ILog _logger;
        
        private readonly ProviderNotificationConfiguration _configuration;

        private readonly IHttpClientWrapper _httpClientWrapper;

        private readonly ExecutionPolicy _executionPolicy;

        public IdamsEmailServiceWrapper(
            ILog logger,
            ProviderApprenticeshipsServiceConfiguration configuration,
            IHttpClientWrapper httpClientWrapper,
            [RequiredPolicy(IdamsExecutionPolicy.Name)]ExecutionPolicy executionPolicy)
        {
            _logger = logger;
            _configuration = configuration.CommitmentNotification;
            _httpClientWrapper = httpClientWrapper;
            _executionPolicy = executionPolicy;
        }

        public virtual async Task<List<string>> GetEmailsAsync(long ukprn)
        {
            var url = string.Format(_configuration.IdamsListUsersUrl, _configuration.DasUserRoleId, ukprn);
            _logger.Info($"Getting 'DAS' emails for provider {ukprn}");
            var result = await GetString(url);
            return ParseIdamsResult(result, ukprn);
        }

        public virtual async Task<List<string>> GetSuperUserEmailsAsync(long ukprn)
        {
            var url = string.Format(_configuration.IdamsListUsersUrl, _configuration.SuperUserRoleId, ukprn);
            _logger.Info($"Getting 'super user' emails for provider {ukprn}");
            var result = GetString(url);
            return ParseIdamsResult(await result, ukprn);

        }

        private List<string> ParseIdamsResult(string jsonResult, long ukprn)
        {
            if (string.IsNullOrWhiteSpace(jsonResult))
            {
                _logger.Debug($"Not possible to parse empty string to {typeof(UserResponse)} for provider: {ukprn}");
                return new List<string>();
            }

            try
            {
                var result = JsonConvert.DeserializeObject<UserResponse>(jsonResult);
                return result.Users.Select(u => u.Email).ToList();
            }
            catch (Exception exception)
            {
                _logger.Info($"Result: {jsonResult}");
                _logger.Error(
                    exception,
                    $"Not possible to parse result to {typeof(UserResponse)} for provider: {ukprn}");
            }

            return new List<string>();
        }

        private async Task<string> GetString(string url)
        {
            var result = string.Empty;
            try
            {
                await _executionPolicy.ExecuteAsync(
                    async () =>
                    {
                        result = await _httpClientWrapper.GetStringAsync(url);
                    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting idams emails");
            }
            return result;
        }
    }
}
