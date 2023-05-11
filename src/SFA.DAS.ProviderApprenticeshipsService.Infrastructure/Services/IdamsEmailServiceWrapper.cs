﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

public class IdamsEmailServiceWrapper : IIdamsEmailServiceWrapper
{
    private readonly ILogger<IdamsEmailServiceWrapper> _logger;
    private readonly ProviderNotificationConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public IdamsEmailServiceWrapper(
        ILogger<IdamsEmailServiceWrapper> logger,
        ProviderNotificationConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public virtual async Task<List<string>> GetEmailsAsync(long ukprn, string identities)
    {
        _logger.LogInformation("Getting emails for provider {Ukprn} for roles {Identities}", ukprn, identities);

        var ids = identities.Split(',');
        var tasks = ids.Select(id => GetStringAsync(string.Format(_configuration.IdamsListUsersUrl, id, ukprn)));
        var results = await Task.WhenAll(tasks);

        return results.SelectMany(result => ParseIdamsResult(result, ukprn)).ToList();
    }

    private static IEnumerable<string> ParseIdamsResult(string jsonResult, long providerId)
    {
        try
        {
            var result = JObject.Parse(jsonResult).SelectToken("result");

            if (result.Type == JTokenType.Array)
            {
                var items = result.ToObject<IEnumerable<UserResponse>>();
                return items.SelectMany(response => response.Emails).ToList();
            }

            var item = result.ToObject<UserResponse>();

            return item?.Emails ?? new List<string>(0);
        }
        catch (JsonSerializationException)
        {
            // Idams query returned no results - { result : ["internal error"] }.
            return Array.Empty<string>();
        }
        catch (Exception exception)
        {
            var resultDescription = string.IsNullOrWhiteSpace(jsonResult) ? "empty string" : $"\"{jsonResult}\"";
            throw new InvalidOperationException($"Not possible to parse {resultDescription} to {typeof(UserResponse)} for provider: {providerId}", exception);
        }
    }

    private async Task<string> GetStringAsync(string url)
    {
        _logger.LogInformation("Querying {Url} for user details", url);

        var httpResponse = await _httpClient.GetAsync(url);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new CustomHttpRequestException(httpResponse.StatusCode, httpResponse.ReasonPhrase);
        }

        return await httpResponse.Content.ReadAsStringAsync();
    }
}

public class CustomHttpRequestException : HttpRequestException
{
    public CustomHttpRequestException(HttpStatusCode statusCode, string reasonPhrase)
        : base($"An unexpected HTTP error occurred due to reason '{reasonPhrase}'.", null, statusCode) { }
}