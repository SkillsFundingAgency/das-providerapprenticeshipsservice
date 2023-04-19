using System;
using System.Collections.Generic;
using System.Security.Principal;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Routing;
using SFA.DAS.Testing;
using Fix = SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Authorization.AuthorizationContextProviderTestsFixture;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Authorization;

[TestFixture]
[Parallelizable]
public class AuthorizationContextProviderTests : FluentTest<Fix>
{
    [Test]
    public void WhenGettingAuthorizationContextAndAllRequiredRouteValuesAreAvailable_ThenShouldReturnAuthorizationContextWithAllValuesSet()
    {
        Run(f => f.SetValidAccountLegalEntityPublicHashedId().SetValidProviderId(), f => f.GetAuthorizationContext(), (f, r) =>
        {
            r.Should().NotBeNull();
            // now that we use the extension method AddProviderPermissionValues() to set the values,
            // ideally we shouldn't be looking for the individual keys like this (as it's peering into a black box)
            // but it's the best alternative for now!
            r.Get<long?>(Fix.ContextKeys.AccountLegalEntityId).Should().Be(f.AccountLegalEntityId);
            r.Get<long?>(Fix.ContextKeys.Ukprn).Should().Be(f.ProviderId);
        });
    }
    
    [Test]
    public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButProviderIdIsNotValid_ThenShouldThrowException()
    {
        Run(f => f.SetValidAccountLegalEntityPublicHashedId().SetInvalidProviderId(),
            f => f.GetAuthorizationContext(),
            (f, a) => a.Should().ThrowExactly<InvalidOperationException>().WithMessage("AuthorizationContextProvider error - Unable to extract ProviderId"));
    }

    [Test]
    public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButProviderIdIsNotAvailable_ThenShouldThrowException()
    {
        Run(f => f.SetValidAccountLegalEntityPublicHashedId(),
            f => f.GetAuthorizationContext(),
            (f, a) => a.Should().ThrowExactly<InvalidOperationException>().WithMessage("AuthorizationContextProvider error - Unable to extract ProviderId"));
    }
}

public class AuthorizationContextProviderTestsFixture
{
    public static class ContextKeys
    {
        public const string AccountLegalEntityId = "AccountLegalEntityId";
        public const string Ukprn = "Ukprn";
    }

    public IAuthorizationContextProvider AuthorizationContextProvider { get; set; }
    public Mock<IHttpContextAccessor> HttpContext { get; set; }
    public Mock<IActionContextAccessorWrapper> ActionContextAccessorWrapper { get; set; }
    public Mock<IEncodingService> EncodingService { get; set; }
    public string AccountLegalEntityPublicHashedIdRouteValue { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string ProviderIdRouteValue { get; set; }
    public long ProviderId { get; set; }
    public RouteData RouteData { get; set; }
    public IQueryCollection QueryParams { get; set; }

    public AuthorizationContextProviderTestsFixture()
    {
        RouteData = new RouteData();
        ActionContextAccessorWrapper = new Mock<IActionContextAccessorWrapper>();
        HttpContext = new Mock<IHttpContextAccessor>();
        ActionContextAccessorWrapper.Setup(c => c.GetRouteData()).Returns(RouteData);
        HttpContext.Setup(c => c.HttpContext.User).Returns(new GenericPrincipal(new Mock<IIdentity>().Object, Array.Empty<string>()));

        EncodingService = new Mock<IEncodingService>();
        AuthorizationContextProvider = new AuthorizationContextProvider(HttpContext.Object,
            EncodingService.Object, 
            Mock.Of<ILogger<AuthorizationContextProvider>>(),
            ActionContextAccessorWrapper.Object);
    }

    public IAuthorizationContext GetAuthorizationContext()
    {
        return AuthorizationContextProvider.GetAuthorizationContext();
    }

    public AuthorizationContextProviderTestsFixture SetValidAccountLegalEntityPublicHashedId()
    {
        AccountLegalEntityPublicHashedIdRouteValue = "ABC123";
        AccountLegalEntityId = 123;
            
        var accountLegalEntityPublicHashedId = new Dictionary<string, StringValues>()
        {
            { RouteDataKeys.EmployerAccountLegalEntityPublicHashedId, new StringValues(AccountLegalEntityPublicHashedIdRouteValue) }
        };
        QueryParams = new QueryCollection(accountLegalEntityPublicHashedId);

        HttpContext.Setup(c => c.HttpContext.Request.Query).Returns(QueryParams);

        EncodingService.Setup(h => h.Decode(AccountLegalEntityPublicHashedIdRouteValue, EncodingType.PublicAccountLegalEntityId)).Returns(AccountLegalEntityId);

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetInvalidHashAccountLegalEntityPublicHashedId()
    {
        AccountLegalEntityPublicHashedIdRouteValue = "AAA";

        var accountLegalEntityPublicHashedId = new Dictionary<string, StringValues>()
        {
            { RouteDataKeys.EmployerAccountLegalEntityPublicHashedId, new StringValues(AccountLegalEntityPublicHashedIdRouteValue) }
        };
        QueryParams = new QueryCollection(accountLegalEntityPublicHashedId);

        HttpContext.Setup(c => c.HttpContext.Request.Query).Returns(QueryParams);

        EncodingService.Setup(h => h.Decode(AccountLegalEntityPublicHashedIdRouteValue, EncodingType.PublicAccountLegalEntityId)).Throws<Exception>();

        return this;
    }
    
    public AuthorizationContextProviderTestsFixture SetValidProviderId()
    {
        ProviderIdRouteValue = "123";
        ProviderId = 123;

        RouteData.Values[RouteDataKeys.ProviderId] = ProviderIdRouteValue;

        return this;
    }

    public AuthorizationContextProviderTestsFixture SetInvalidProviderId()
    {
        ProviderIdRouteValue = "Skunk";

        RouteData.Values[RouteDataKeys.ProviderId] = ProviderIdRouteValue;

        return this;
    }
}