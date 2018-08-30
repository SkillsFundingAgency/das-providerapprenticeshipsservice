using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.Messages
{
    [TestFixture]
    public class WhenExtractingMessages
    {
        [TestCase("Banner||Field", "Banner")]
        [TestCase("Same for both", "Same for both")]
        [TestCase("|Same| |for| |both|", "|Same| |for| |both|")]
        [TestCase("", "")]
        public void ThenBanerMessageIsExtracted(string source, string expected)
        {
            var bannerMessage = ValidationMessage.ExtractBannerMessage(source);
            Assert.AreEqual(expected, bannerMessage);
        }

        [TestCase("Banner||Field", "Field")]
        [TestCase("Same for both", "Same for both")]
        [TestCase("|Same| |for| |both|", "|Same| |for| |both|")]
        [TestCase("", "")]
        public void ThenFieldMessageIsExtracted(string source, string expected)
        {
            var bannerMessage = ValidationMessage.ExtractFieldMessage(source);
            Assert.AreEqual(expected, bannerMessage);
        }
    }
}
