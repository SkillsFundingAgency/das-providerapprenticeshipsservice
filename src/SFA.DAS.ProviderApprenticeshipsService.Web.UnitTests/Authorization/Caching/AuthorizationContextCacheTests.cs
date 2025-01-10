using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Caching;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Authorization.Caching;

[TestFixture]
[Parallelizable]
public class AuthorizationContextCacheTests : AuthorizationContextCacheTestsFixture
{
    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContext_ThenShouldReturnAuthorizationContext()
    {
        var fixture = new AuthorizationContextCacheTestsFixture();
        var result = fixture.GetAuthorizationContext();
        result.SingleOrDefault().Should().NotBeNull();
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContext_ThenShouldGetAuthorizationContextFromAuthorizationContextProvider()
    {
        var fixture = new AuthorizationContextCacheTestsFixture();
        fixture.GetAuthorizationContext();
        fixture.AuthorizationContextProvider.Verify(p => p.GetAuthorizationContext(), Times.Once);
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContextMultipleTimes_ThenShouldGetAuthorizationContextFromAuthorizationContextProviderOnce()
    {
        var fixture = new AuthorizationContextCacheTestsFixture();
        fixture.GetAuthorizationContext();
        fixture.AuthorizationContextProvider.Verify(p => p.GetAuthorizationContext(), Times.Once);
    }

    [Test]
    public void GetAuthorizationContext_WhenGettingAuthorizationContextMultipleTimes_ThenShouldReturnSameAuthorizationContext()
    {
        var fixture = new AuthorizationContextCacheTestsFixture();
        var result = fixture.GetAuthorizationContext();
        result.ForEach(c => c.Should().Be(result.First()));
    }
}

public class AuthorizationContextCacheTestsFixture
{
    public Mock<IAuthorizationContextProvider> AuthorizationContextProvider { get; set; }
    public IAuthorizationContextProvider AuthorizationContextCache { get; set; }

    public AuthorizationContextCacheTestsFixture()
    {
        AuthorizationContextProvider = new Mock<IAuthorizationContextProvider>();
        AuthorizationContextCache = new AuthorizationContextCache(AuthorizationContextProvider.Object);

        AuthorizationContextProvider.Setup(p => p.GetAuthorizationContext()).Returns(() => new AuthorizationContext());
    }

    public List<IAuthorizationContext> GetAuthorizationContext(int count = 1)
    {
        var authorizationContexts = new List<IAuthorizationContext>();

        for (var i = 0; i < count; i++)
        {
            authorizationContexts.Add(AuthorizationContextCache.GetAuthorizationContext());
        }

        return authorizationContexts;
    }
}