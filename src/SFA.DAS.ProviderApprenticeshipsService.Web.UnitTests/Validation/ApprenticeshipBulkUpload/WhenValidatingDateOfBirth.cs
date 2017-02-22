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
    public class WhenValidatingDateOfBirth
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
                StartDate = new DateTimeViewModel(new DateTime(2017, 06, 20)),
                EndDate = new DateTimeViewModel(new DateTime(2018, 05, 15)),
                Cost = "1234"
            };
        }

        [Test]
        public void ShouldBeInvalidIfDateValueIsNull()
        {
            var result = _validator.Validate(_validModel);

            result.Errors.Count.Should().Be(1);
        }
    }
}