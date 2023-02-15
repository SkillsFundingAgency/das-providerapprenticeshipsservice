using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.GetRoatpBetaProviderService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Services
{
    [TestFixture]
    public class GetRoatpBetaProviderServiceTests
    {
        private GetRoatpBetaProviderService _service;
        private RoatpCourseManagementWebConfiguration _configuration;
        private int ukprnPresent = 12345678;
        private int ukprnOther = 87654321;

        [SetUp]
        public void Before_Each_Test()
        {
            _configuration = new RoatpCourseManagementWebConfiguration
            {
                ProviderFeaturesConfiguration = new ProviderFeaturesConfiguration
                {
                    FeatureToggles = new List<ProviderFeatureToggle> { new ProviderFeatureToggle {
                            Feature = "CourseManagement",
                            IsEnabled = true,
                            Whitelist = new List<ProviderFeatureToggleWhitelistItem>
                            {
                                new ProviderFeatureToggleWhitelistItem {Ukprn = ukprnPresent}
                            }
                        }
                    }
                }
            };
        }

        [Test]
        public void IsUkprnEnabled_UkprnPresent_ReturnsTrue()
        {
            _service = new GetRoatpBetaProviderService(_configuration);
            var result = _service.IsUkprnEnabled(ukprnPresent);
            Assert.IsTrue(result);
        }

        [Test]
        public void IsUkprnEnabled_DifferentFeatureName_ReturnsFalse()
        {
            _configuration.ProviderFeaturesConfiguration.FeatureToggles[0].Feature = "DifferentName";
            _service = new GetRoatpBetaProviderService(_configuration);
            var result = _service.IsUkprnEnabled(ukprnPresent);
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUkprnEnabled_FeatureNotEnabled_ReturnsFalse()
        {
            _configuration.ProviderFeaturesConfiguration.FeatureToggles[0].IsEnabled = false;
            _service = new GetRoatpBetaProviderService(_configuration);
            var result = _service.IsUkprnEnabled(ukprnPresent);
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUkprnEnabled_NullWhitelist_ReturnsFalse()
        {
            _configuration.ProviderFeaturesConfiguration.FeatureToggles[0].Whitelist=null;
            _service = new GetRoatpBetaProviderService(_configuration);
            var result = _service.IsUkprnEnabled(ukprnPresent);
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUkprnEnabled_EmptyWhitelist_ReturnsFalse()
        {
            _configuration.ProviderFeaturesConfiguration.FeatureToggles[0].Whitelist = new List<ProviderFeatureToggleWhitelistItem>();
            _service = new GetRoatpBetaProviderService(_configuration);
            var result = _service.IsUkprnEnabled(ukprnPresent);
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUkprnEnabled_UkprnNotPresent_ReturnsFalse()
        {
            _service = new GetRoatpBetaProviderService(_configuration);
            var result = _service.IsUkprnEnabled(ukprnOther);
            Assert.IsFalse(result);
        }
    }
}
