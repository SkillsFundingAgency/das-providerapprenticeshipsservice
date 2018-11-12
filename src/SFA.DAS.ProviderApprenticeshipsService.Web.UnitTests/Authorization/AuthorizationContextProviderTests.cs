using System;
using System.Web;
using System.Web.Routing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
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
                //todo: now that we use the extension method to set the values, we probably shouldn't be looking for the individual keys
                // but we're probably going to switch to magic marker anyway
                r.Get<long?>(Fix.ContextKeys.AccountLegalEntityId).Should().Be(f.AccountLegalEntityId);
                r.Get<long?>(Fix.ContextKeys.Ukprn).Should().Be(f.ProviderId);
            });
        }

        #region Invalid AccountLegalEntityPublicHashedId

        [Test]
        public void WhenGettingAuthorizationContextAndRequiredRouteValuesAreAvailableButAccountLegalEntityPublicHashedIdIsNotAValidHash_ThenShouldThrowException()
        {
            Run(f => f.SetInvalidHashAccountLegalEntityPublicHashedId().SetValidProviderId(),
                f => f.GetAuthorizationContext(),
                (f, r) => r.Should().Throw<Exception>());
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

        public AuthorizationContextProviderTestsFixture()
        {
            RouteData = new RouteData();

            HttpContext = new Mock<HttpContextBase>();
            HttpContext.Setup(c => c.Request.RequestContext.RouteData).Returns(RouteData);

            PublicHashingService = new Mock<IPublicHashingService>();
            AuthorizationContextProvider = new AuthorizationContextProvider(HttpContext.Object, PublicHashingService.Object);
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

            RouteData.Values[RouteDataKeys.AccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedIdRouteValue;

            PublicHashingService.Setup(h => h.DecodeValue(AccountLegalEntityPublicHashedIdRouteValue)).Returns(AccountLegalEntityId);

            return this;
        }

        public AuthorizationContextProviderTestsFixture SetInvalidHashAccountLegalEntityPublicHashedId()
        {
            AccountLegalEntityPublicHashedIdRouteValue = "AAA";

            RouteData.Values[RouteDataKeys.AccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedIdRouteValue;

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
