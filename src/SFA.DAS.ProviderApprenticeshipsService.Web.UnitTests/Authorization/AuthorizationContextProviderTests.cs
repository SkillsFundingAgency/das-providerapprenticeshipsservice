using System;
using System.Collections.Specialized;
using System.Security.Principal;
using FluentAssertions;
using FluentAssertions.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Routing;
using SFA.DAS.Testing;
using Fix = SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Authorization.AuthorizationContextProviderTestsFixture;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Authorization
{
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

        #region Invalid ProviderId

        [Test]
        public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButProviderIdIsNotValid_ThenShouldThrowException()
        {
            Run(f => f.SetValidAccountLegalEntityPublicHashedId().SetInvalidProviderId(),
                f => f.GetAuthorizationContext(),
                (f, a) => a.Should().ThrowExactly<Exception>().WithMessage("AuthorizationContextProvider error - Unable to extract ProviderId"));
        }

        [Test]
        public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButProviderIdIsNotAvailable_ThenShouldThrowException()
        {
            Run(f => f.SetValidAccountLegalEntityPublicHashedId(),
                f => f.GetAuthorizationContext(),
                (f, a) => a.Should().ThrowExactly<Exception>().WithMessage("AuthorizationContextProvider error - Unable to extract ProviderId"));
        }

        #endregion Invalid ProviderId
    }

    public class AuthorizationContextProviderTestsFixture
    {
        public static class ContextKeys
        {
            public const string AccountLegalEntityId = "AccountLegalEntityId";
            public const string Ukprn = "Ukprn";
        }

        public IAuthorizationContextProvider AuthorizationContextProvider { get; set; }
        public Mock<HttpContext> HttpContext { get; set; }
        public Mock<IAccountLegalEntityPublicHashingService> AccountLegalEntityPublicHashingService { get; set; }
        public string AccountLegalEntityPublicHashedIdRouteValue { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string ProviderIdRouteValue { get; set; }
        public long ProviderId { get; set; }
        public RouteData RouteData { get; set; }
        public IQueryCollection QueryParams { get; set; }

        public AuthorizationContextProviderTestsFixture()
        {
            RouteData = new RouteData();
            QueryParams = new QueryCollection();

            HttpContext = new Mock<HttpContext>();
            HttpContext.Setup(c => c.Request.HttpContext.GetRouteData()).Returns(RouteData);
            HttpContext.Setup(c => c.Request.Query).Returns(QueryParams);
            HttpContext.Setup(c => c.User).Returns(new GenericPrincipal(new Mock<IIdentity>().Object, new string[0]));

            AccountLegalEntityPublicHashingService = new Mock<IAccountLegalEntityPublicHashingService>();
            AuthorizationContextProvider = new AuthorizationContextProvider(HttpContext.Object, 
                AccountLegalEntityPublicHashingService.Object, 
                Mock.Of<ILogger<AuthorizationContextProvider>>(),
                Mock.Of<IActionContextAccessor>());
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            return AuthorizationContextProvider.GetAuthorizationContext();
        }

        #region Set AccountLegalEntityId

        public AuthorizationContextProviderTestsFixture SetValidAccountLegalEntityPublicHashedId()
        {
            AccountLegalEntityPublicHashedIdRouteValue = "ABC123";
            AccountLegalEntityId = 123;

            QueryParams[RouteDataKeys.EmployerAccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedIdRouteValue;

            AccountLegalEntityPublicHashingService.Setup(h => h.DecodeValue(AccountLegalEntityPublicHashedIdRouteValue)).Returns(AccountLegalEntityId);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidHashAccountLegalEntityPublicHashedId()
        {
            AccountLegalEntityPublicHashedIdRouteValue = "AAA";

            QueryParams[RouteDataKeys.EmployerAccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedIdRouteValue;

            AccountLegalEntityPublicHashingService.Setup(h => h.DecodeValue(AccountLegalEntityPublicHashedIdRouteValue)).Throws<Exception>();

            return this;
        }

        #endregion Set AccountLegalEntityId

        #region Set ProviderId

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

        #endregion Set ProviderId        
    }
}
