using FluentAssertions.Execution;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Account;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Models.Account
{
    public class WhenBuildingChangeOfDetailsViewModel
    {
        [TestCase("prod")]
        [TestCase("prd")]
        public void Then_The_ProfileService_Link_Is_Correct_For_Production_Environment(string environment)
        {
            var actual = new ChangeOfDetailsViewModel(environment);

            using (new AssertionScope())
            {
                Assert.That(actual.ProfilePageLink, Is.Not.Null);
                Assert.That(actual.ProfilePageLink, Is.EqualTo("https://profile.signin.education.gov.uk/"));
            }
        }

        [Test]
        public void Then_The_ProfileService_Link_Is_Correct_For_Pre_Production_Environment()
        {
            var actual = new ChangeOfDetailsViewModel("preprod");

            using (new AssertionScope())
            {
                Assert.That(actual.ProfilePageLink, Is.Not.Null);
                Assert.That(actual.ProfilePageLink, Is.EqualTo("https://pp-profile.signin.education.gov.uk/"));
            }
        }

        [TestCase("")]
        [TestCase(null)]
        public void Then_The_ProfileService_Link_Is_Correct_When_Environment_Is_Null(string env)
        {
            var actual = new ChangeOfDetailsViewModel(env);

            using (new AssertionScope())
            {
                Assert.That(actual.ProfilePageLink, Is.Not.Null);
                Assert.That(actual.ProfilePageLink, Is.EqualTo("https://test-profile.signin.education.gov.uk/"));
            }
        }
    }
}
