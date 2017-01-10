using FluentAssertions;

using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation
{
    [TestFixture]
    public class WhenValidatingApprenticeshipBulkUploadViewModel
    {
        private readonly ApprenticeshipBulkUploadValidator _validator = new ApprenticeshipBulkUploadValidator();
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }

        [Test]
        public void NamesShouldNotBeEmpty()
        {
            _validModel.FirstName = "  ";
            _validModel.LastName = " ";

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(2);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter given names that are no longer than 100 characters");
            result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("You must enter a family name that's no longer than 100 characters");
        }

        [Test]
        public void TestNamesNotNull()
        {
            _validModel.FirstName = null;
            _validModel.LastName = null;

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(2);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("The Given names must be entered");
            result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("The Family name must be entered");
        }
    }
}