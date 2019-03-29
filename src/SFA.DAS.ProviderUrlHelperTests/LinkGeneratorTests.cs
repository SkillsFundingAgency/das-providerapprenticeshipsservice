using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.AutoConfiguration;
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
            AutoConfigurationServiceMock = new Mock<IAutoConfigurationService>();

            ProviderUrlConfiguration = new ProviderUrlConfiguration();

            AutoConfigurationServiceMock.Setup(acs => acs.Get<ProviderUrlConfiguration>()).Returns(ProviderUrlConfiguration);
        }

        public ProviderUrlConfiguration ProviderUrlConfiguration { get; }

        public Mock<IAutoConfigurationService> AutoConfigurationServiceMock { get; }
        public IAutoConfigurationService AutoConfigurationService => AutoConfigurationServiceMock.Object;


        public LinkGeneratorTestFixtures WithProviderApprenticeshipServiceBaseUrl(string baseUrl)
        {
            ProviderUrlConfiguration.ProviderApprenticeshipServiceBaseUrl = baseUrl;
            return this;
        }

        public string GetProviderApprenticeshipServiceLink(string path)
        {
            var linkGenerator = new LinkGenerator(AutoConfigurationService);
            return linkGenerator.ProviderApprenticeshipServiceLink(path);
        }
    }
}
