using FluentAssertions;

using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation
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
                                  StartDate = new DateTimeViewModel(1, 5, 2200),
                                  EndDate= new DateTimeViewModel(1, 5, 2200),
                                  TrainingCode = "5",
                                  DateOfBirth = new DateTimeViewModel(5, 9, 1882),
                                  NINumber = "SE000NI00NUKBER"
                              };
        }

        [Test]
        public void TestValidationWithEmptyModel()
        {
            var result = _validator.Validate(new ApprenticeshipViewModel());
            result.IsValid.Should().BeFalse();
            result.Errors.Count.ShouldBeEquivalentTo(9);
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
