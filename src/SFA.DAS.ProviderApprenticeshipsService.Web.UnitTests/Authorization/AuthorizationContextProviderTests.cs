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

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Authorization
{
    [TestFixture]
    [Parallelizable]
    public class AuthorizationContextProviderTests : FluentTest<AuthorizationContextProviderTestsFixture>
    {
        [Test]
        public void GetAuthorizationContext_WhenGettingAuthorizationContextAndAccountLegalEntityPublicHashedIdExistsAndProviderIdExists_ThenShouldReturnAuthorizationContextWithAccountLegalEntityIdAndProviderId()
        {
            Run(f => f.SetValidAccountLegalEntityPublicHashedId().SetValidProviderId(), f => f.GetAuthorizationContext(), (f, r) =>
            {
                r.Should().NotBeNull();
                r.Get<long?>("AccountLegalEntityId").Should().Be(f.AccountLegalEntityId);
                r.Get<long?>("ProviderId").Should().Be(f.AccountLegalEntityId);
            });
        }

        //[Test]
        //public void GetAuthorizationContext_WhenGettingAuthorizationContextAndAccountIdDoesNotExistAndUserIsNotAuthenticated_ThenShouldReturnAuthroizationContextWithoutAccountIdAndUserRefValues()
        //{
        //    Run(f => f.SetNoAccountId().SetUnauthenticatedUser(), f => f.GetAuthorizationContext(), (f, r) =>
        //    {
        //        r.Should().NotBeNull();
        //        r.Get<string>("AccountHashedId").Should().BeNull();
        //        r.Get<long?>("AccountId").Should().BeNull();
        //        r.Get<Guid?>("UserRef").Should().BeNull();
        //    });
        //}

        //[Test]
        //public void GetAuthorizationContext_WhenGettingAuthorizationContextAndAccountIdExistsAndIsInvalid_ThenShouldThrowUnauthorizedAccessException()
        //{
        //    Run(f => f.SetInvalidAccountId(), f => f.GetAuthorizationContext(), (f, r) => r.Should().Throw<UnauthorizedAccessException>());
        //}

        //[Test]
        //public void GetAuthorizationContext_WhenGettingAuthorizationContextAndUserIsAuthenticatedAndUserRefIsInvalid_ThenShouldThrowUnauthorizedAccessException()
        //{
        //    Run(f => f.SetInvalidAccountId().SetInvalidUserRef(), f => f.GetAuthorizationContext(), (f, r) => r.Should().Throw<UnauthorizedAccessException>());
        //}
    }

    public class AuthorizationContextProviderTestsFixture
    {
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

        public AuthorizationContextProviderTestsFixture SetInvalidAccountLegalEntityPublicHashedId()
        {
            AccountLegalEntityPublicHashedIdRouteValue = "AAA";

            RouteData.Values[RouteDataKeys.AccountLegalEntityPublicHashedId] = AccountLegalEntityPublicHashedIdRouteValue;

            PublicHashingService.Setup(h => h.DecodeValue(AccountLegalEntityPublicHashedIdRouteValue)).Throws<Exception>();

            return this;
        }

        //public AuthorizationContextProviderTestsFixture SetNoAccountLegalEntityPublicHashedId()
        //{
        //    return this;
        //}

        #endregion Set AccountLegalEntityId

        #region Set ProviderId

        public AuthorizationContextProviderTestsFixture SetValidProviderId()
        {
            ProviderIdRouteValue = "123";
            ProviderId = 123;

            RouteData.Values[RouteDataKeys.ProviderId] = ProviderIdRouteValue;

            return this;
        }

        #endregion Set ProviderId
    }
}
