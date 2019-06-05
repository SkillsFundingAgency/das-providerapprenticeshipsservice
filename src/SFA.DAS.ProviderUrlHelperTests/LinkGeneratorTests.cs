using NUnit.Framework;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderUrlHelperTests
{
    [TestFixture]
    public class LinkGeneratorTests
    {
        [TestCase("base", "path", "base/path")]
        [TestCase("base/", "path", "base/path")]
        [TestCase("base", "/path", "base/path")]
        [TestCase("base/", "/path", "base/path")]
        public void ProviderCommitmentsLink_(string providerApprenticeshipServiceUrl, string path, string expectedUrl)
        {
            var fixtures = new LinkGeneratorTestFixtures()
                .WithProviderApprenticeshipServiceBaseUrl(providerApprenticeshipServiceUrl);

            var actualUrl = fixtures.GetProviderApprenticeshipServiceLink(path);

            Assert.AreEqual(expectedUrl, actualUrl);
        }
    }
    public class LinkGeneratorTestFixtures
    {
        public LinkGeneratorTestFixtures()
        {
            ProviderUrlConfiguration = new ProviderUrlConfiguration();
        }

        public ProviderUrlConfiguration ProviderUrlConfiguration { get; }

        public LinkGeneratorTestFixtures WithProviderApprenticeshipServiceBaseUrl(string baseUrl)
        {
            ProviderUrlConfiguration.ProviderApprenticeshipServiceBaseUrl = baseUrl;
            return this;
        }

        public string GetProviderApprenticeshipServiceLink(string path)
        {
            var linkGenerator = new LinkGenerator(ProviderUrlConfiguration);
            return linkGenerator.ProviderApprenticeshipServiceLink(path);
        }
    }
}
