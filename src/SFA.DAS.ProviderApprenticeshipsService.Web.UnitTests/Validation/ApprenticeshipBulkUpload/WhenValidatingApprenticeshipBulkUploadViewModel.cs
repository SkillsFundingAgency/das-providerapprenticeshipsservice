using System;
using FluentAssertions;

using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingApprenticeshipBulkUploadViewModel
    {
        private readonly ApprenticeshipBulkUploadValidator _validator = new ApprenticeshipBulkUploadValidator(new BulkUploadApprenticeshipValidationText());
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel
            {
                ULN = "1001234567",
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                DateOfBirth = new DateTimeViewModel(DateTime.Now.AddYears(-16)),
                StartDate = new DateTimeViewModel(new DateTime(2017, 06, 20)),
                EndDate = new DateTimeViewModel(new DateTime(2018, 05, 15)),
                Cost = "1234"
            };
        }

        [Test]
        public void NamesShouldNotBeEmpty()
        {
            _validModel.FirstName = "  ";
            _validModel.LastName = " ";

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(2);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter <strong>Given names</strong> that are no longer than 100 characters");
            result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("You must enter a <strong>Family name</strong> that's no longer than 100 characters");
        }

        [Test]
        public void TestNamesNotNull()
        {
            _validModel.FirstName = null;
            _validModel.LastName = null;

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(2);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter <strong>Given names</strong> that are no longer than 100 characters");
            result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("You must enter a <strong>Family name</strong> that's no longer than 100 characters");
        }
    }
}