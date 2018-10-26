using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.AcademicYear;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture()]
    public class WhenValidatingStartDate : ApprenticeshipValidationTestBase
    {
        [Test]
        public void IfTheTrainingProgrammeIsValidThenShouldPassValidation()
        {
            //Arrange
            ValidModel.TrainingCode = "TESTCOURSE";
            ValidModel.StartDate = new DateTimeViewModel(1, 6, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void IfTheTrainingProgrammeHasNotStartedThenShouldFailValidation()
        {
            //Arrange
            ValidModel.TrainingCode = "TESTCOURSE";
            ValidModel.StartDate = new DateTimeViewModel(1, 4, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors[0].ErrorMessage.EndsWith("after 04 2018"));
        }

        [Test]
        public void IfTheTrainingProgrammeHasExpiredThenShouldFailValidation()
        {
            //Arrange
            ValidModel.TrainingCode = "TESTCOURSE";
            ValidModel.StartDate = new DateTimeViewModel(1, 8, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors[0].ErrorMessage.EndsWith("before 08 2018"));
        }

        [Test]
        public void IfTrainingCodeIsNotSuppliedThenShouldNotCheckCourseValidity()
        {
            //Arrange
            ValidModel.TrainingCode = "";
            ValidModel.StartDate = new DateTimeViewModel(1, 4, 2018);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            Assert.IsTrue(result.IsValid);
            MockMediator.Verify(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()), Times.Never);
        }

        [Test]
        public void AndStartDateWithinFundingPeriodThenValid()
        {
            //Arrange
            MockAcademicYearValidator
                .Setup(validator => validator.Validate(It.IsAny<DateTime>()))
                .Returns(AcademicYearValidationResult.Success);

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void AndStartDateNotWithinFundingPeriodThenInvalid()
        {
            //Arrange
            MockAcademicYearValidator
                .Setup(validator => validator.Validate(It.IsAny<DateTime>()))
                .Returns(AcademicYearValidationResult.NotWithinFundingPeriod);
            ValidModel.StartDate = new DateTimeViewModel(DateTime.Parse("2018-06-30"));
            ValidModel.EndDate = new DateTimeViewModel(DateTime.Parse("2020-05-10"));

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().MatchRegex("The earliest start date you can use is [0-9]{2} [0-9]{4}");
            result.Errors.Single().ErrorCode.Should().Be("AcademicYear_01");
        }

        [Test]
        public void AndStartDateIsBeforeAcademicYearAndBeforeTrainingStartThenOnlyHasStartDateError()
        {
            //Arrange
            MockAcademicYearValidator
                .Setup(validator => validator.Validate(It.IsAny<DateTime>()))
                .Returns(AcademicYearValidationResult.NotWithinFundingPeriod);
            ValidModel.StartDate = new DateTimeViewModel(DateTime.Parse("2017-04-30"));
            ValidModel.EndDate = new DateTimeViewModel(DateTime.Parse("2020-05-10"));
            ValidModel.IsPaidForByTransfer = true;

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().MatchRegex("The earliest start date you can use is [0-9]{2} [0-9]{4}");
            result.Errors.Single().ErrorCode.Should().Be("AcademicYear_01");
        }
    }
}
