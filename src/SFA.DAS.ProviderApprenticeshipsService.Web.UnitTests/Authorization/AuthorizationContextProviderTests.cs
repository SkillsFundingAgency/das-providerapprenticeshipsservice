using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.NLog.Logger;
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

        #region Invalid AccountLegalEntityPublicHashedId

        [Test]
        public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButAccountLegalEntityPublicHashedIdIsNotAValidHash_ThenShouldReturnAuthorizationContextWithNullAccountLegalEntityId()
        {
            Run(f => f.SetInvalidHashAccountLegalEntityPublicHashedId().SetValidProviderId(),
                f => f.GetAuthorizationContext(),
                (f, r) =>
                {
                    r.Should().NotBeNull();
                    r.Get<long?>(Fix.ContextKeys.AccountLegalEntityId).Should().BeNull();
                    r.Get<long?>(Fix.ContextKeys.Ukprn).Should().Be(f.ProviderId);
                });
        }

        [Test]
        public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButAccountLegalEntityPublicHashedIdIsNotAvailable_ThenShouldReturnAuthorizationContextWithAllValuesSetExceptAccountLegalEntityId()
        {
            Run(f => f.SetValidProviderId(),
                f => f.GetAuthorizationContext(),
                (f, r) =>
                {
                    r.Should().NotBeNull();
                    r.Get<long?>(Fix.ContextKeys.Ukprn).Should().Be(f.ProviderId);

                    var exists = r.TryGet<long?>(Fix.ContextKeys.AccountLegalEntityId, out var value);
                    exists.Should().BeTrue();
                    value.Should().BeNull();
                });
        }

        #endregion Invalid AccountLegalEntityPublicHashedId

        #region Invalid ProviderId

        [Test]
        public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButProviderIdIsNotValid_ThenShouldReturnAuthorizationContextWithAllValuesSetExceptProviderId()
        {
            Run(f => f.SetValidAccountLegalEntityPublicHashedId().SetInvalidProviderId(),
                f => f.GetAuthorizationContext(),
                (f, r) =>
                {
                    r.Should().NotBeNull();
                    r.Get<long?>(Fix.ContextKeys.AccountLegalEntityId).Should().Be(f.AccountLegalEntityId);

                    var exists = r.TryGet<long?>(Fix.ContextKeys.Ukprn, out var value);
                    exists.Should().BeTrue();
                    value.Should().BeNull();
                });
        }

        [Test]
        public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButProviderIdIsNotAvailable_ThenShouldReturnAuthorizationContextWithAllValuesSetExceptProviderId()
        {
            Run(f => f.SetValidAccountLegalEntityPublicHashedId(),
                f => f.GetAuthorizationContext(),
                (f, r) =>
                {
                    r.Should().NotBeNull();
                    r.Get<long?>(Fix.ContextKeys.AccountLegalEntityId).Should().Be(f.AccountLegalEntityId);

                    var exists = r.TryGet<long?>(Fix.ContextKeys.Ukprn, out var value);
                    exists.Should().BeTrue();
                    value.Should().BeNull();
                });
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
        public Mock<HttpContextBase> HttpContext { get; set; }
        public Mock<IPublicHashingService> PublicHashingService { get; set; }
        public string AccountLegalEntityPublicHashedIdRouteValue { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string ProviderIdRouteValue { get; set; }
        public long ProviderId { get; set; }
        public RouteData RouteData { get; set; }
        public NameValueCollection Params { get; set; }

        public AuthorizationContextProviderTestsFixture()
        {
            RouteData = new RouteData();
            Params = new NameValueCollection();

            HttpContext = new Mock<HttpContextBase>();
            HttpContext.Setup(c => c.Request.RequestContext.RouteData).Returns(RouteData);
            HttpContext.Setup(c => c.Request.Params).Returns(Params);

            PublicHashingService = new Mock<IPublicHashingService>();
            AuthorizationContextProvider = new AuthorizationContextProvider(HttpContext.Object, PublicHashingService.Object, Mock.Of<ILog>());
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

            Params[RouteDataKeys.EmployerAccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedIdRouteValue;

            PublicHashingService.Setup(h => h.DecodeValue(AccountLegalEntityPublicHashedIdRouteValue)).Returns(AccountLegalEntityId);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidHashAccountLegalEntityPublicHashedId()
        {
            AccountLegalEntityPublicHashedIdRouteValue = "AAA";

            Params[RouteDataKeys.EmployerAccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedIdRouteValue;

            PublicHashingService.Setup(h => h.DecodeValue(AccountLegalEntityPublicHashedIdRouteValue)).Throws<Exception>();

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
