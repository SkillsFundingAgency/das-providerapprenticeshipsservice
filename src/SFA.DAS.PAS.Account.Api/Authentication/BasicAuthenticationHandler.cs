﻿using SFA.DAS.PAS.Account.Api.Authorization;

namespace SFA.DAS.PAS.Account.Api.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "username"),
            new Claim(ClaimTypes.Name, "username"),
            new Claim(ClaimTypes.Role, ApiRoles.ReadUserSettings),
            new Claim(ClaimTypes.Role, ApiRoles.ReadAccountUsers)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}