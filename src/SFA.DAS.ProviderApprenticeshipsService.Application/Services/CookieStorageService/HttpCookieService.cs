﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;

public class HttpCookieService<T> : ICookieService<T>
{
    private readonly IDataProtector _protector;

    public HttpCookieService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("SFA.DAS.ProviderApprenticeshipsService.Services.HttpCookieService");
    }

    public void Create(IHttpContextAccessor contextAccessor, string name, T content, int expireDays)
    {
        var cookieContent = JsonConvert.SerializeObject(content);

        var encodedContent = Convert.ToBase64String(_protector.Protect(new UTF8Encoding().GetBytes(cookieContent)));

        contextAccessor.HttpContext.Response.Cookies.Append(name, encodedContent, new CookieOptions
        {
            Expires = DateTime.Now.AddDays(expireDays),
            Secure = true,
            HttpOnly = true,
        });
    }

    public void Update(IHttpContextAccessor contextAccessor, string name, T content)
    {
        var cookie = contextAccessor.HttpContext.Request.Cookies[name];

        if (cookie != null)
        {
            var cookieContent = JsonConvert.SerializeObject(content);

            var encodedContent = Convert.ToBase64String(_protector.Protect(new UTF8Encoding().GetBytes(cookieContent)));
            contextAccessor.HttpContext.Response.Cookies.Append(name, encodedContent);
        }
    }

    public void Delete(IHttpContextAccessor contextAccessor, string name)
    {
        if (contextAccessor.HttpContext.Request.Cookies[name] != null)
        {
            contextAccessor.HttpContext.Response.Cookies.Delete(name);
        }
    }

    public T Get(IHttpContextAccessor contextAccessor, string name)
    {
        if (contextAccessor.HttpContext.Request.Cookies[name] == null)
            return default;

        var base64EncodedBytes = Convert.FromBase64String(contextAccessor.HttpContext.Request.Cookies[name]);
        return JsonConvert.DeserializeObject<T>(new UTF8Encoding().GetString(_protector.Unprotect(base64EncodedBytes)));
    }
}