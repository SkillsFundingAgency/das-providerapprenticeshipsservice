using FluentAssertions;

using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests
{
    [TestFixture]
    public class WhenValidatingApprenticeshipViewModelForApproval
    {

        private ApprenticeshipViewModelApproveValidator _validator = new ApprenticeshipViewModelApproveValidator();
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel
                              {
                                  FirstName = "First Name",
                                  LastName = "Last Name",
                                  ULN = "ULN",
                                  Cost = "COST",
                                  StartMonth = 5,
                                  StartYear = 2005,
                                  EndMonth = 5,
                                  EndYear = 2015,
                                  TrainingCode = "5",
                                  DateOfBirthDay = 5,
                                  DateOfBirthMonth = 9,
                                  DateOfBirthYear = 1882,
                                  NINumber = "SE000NI00NUKBER"
                              };
        }

        [Test]
        public void TestValidationWithEmptyModel()
        {
            var result = _validator.Validate(new ApprenticeshipViewModel());
            result.IsValid.Should().BeFalse();
            result.Errors.Count.ShouldBeEquivalentTo(13);
        }

        [Test]
        public void TestValidationWithValidModel()
        {
            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeTrue();
            result.Errors.Count.ShouldBeEquivalentTo(0);
            result.Errors.Count.ShouldBeEquivalentTo(0);
        }
    }
}
