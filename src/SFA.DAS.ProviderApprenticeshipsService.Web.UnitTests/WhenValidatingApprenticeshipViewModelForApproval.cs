using FluentAssertions;
using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests
{
    [TestFixture]
    public class WhenValidatingApprenticeshipViewModelForApproval
    {

        private readonly ApprenticeshipViewModelApproveValidator _validator = new ApprenticeshipViewModelApproveValidator();
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
                                  ProgType = 25,
                                  DateOfBirth = new DateTimeViewModel(5, 9, 1882),
                                  NINumber = "SE000NI00NUKBER"
                              };
        }

        [Test]
        public void TestValidationWithEmptyModel()
        {
            var result = _validator.Validate(new ApprenticeshipViewModel());
            result.IsValid.Should().BeFalse();
            result.Errors.Count.ShouldBeEquivalentTo(7);
        }

        [Test]
        public void TestValidationWithValidModel()
        {
            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeTrue();
            result.Errors.Count.ShouldBeEquivalentTo(0);
        }

        [TestCase(25, "")]
        [TestCase(25, null)]
        [TestCase(25, "  ")]

        [TestCase(2, "2")]
        [TestCase(3, "2")]
        [TestCase(20, "2")]
        [TestCase(21, "2")]
        [TestCase(22, "2")]
        [TestCase(23, "2")]
        public void TestTrainingCodeValidation(int? progType, string trainingCode)
        {
            _validModel.ProgType = progType;
            _validModel.TrainingCode = trainingCode;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeFalse();
            result.Errors.Count.ShouldBeEquivalentTo(1);

        }
    }
}
