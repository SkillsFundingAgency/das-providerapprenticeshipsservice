using System;
using NUnit.Framework;

#if NETCOREAPP
using SFA.DAS.ProviderUrlHelperTests.Core;
#elif NETFRAMEWORK
using SFA.DAS.ProviderUrlHelperTests.Framework;
#endif

namespace SFA.DAS.ProviderUrlHelperTests
{
    [TestFixture]
    public class UrlHelperExtensionsTests
    {
        [TestCase("base", "path", "base/path")]
        [TestCase("base/", "path", "base/path")]
        [TestCase("base", "/path", "base/path")]
        [TestCase("base/", "/path", "base/path")]
        public void ProviderCommitmentsLink_(string providerApprenticeshipServiceUrl, string path, string expectedUrl)
        {
            var fixtures = new UrlHelperExtensionsTestsFixtures()
                .WithProviderApprenticeshipServiceBaseUrl(providerApprenticeshipServiceUrl);

            var actualUrl = fixtures.GetProviderfApprenticeshipServiceLink(path);

            Assert.AreEqual(expectedUrl, actualUrl);
        }
    }

}
