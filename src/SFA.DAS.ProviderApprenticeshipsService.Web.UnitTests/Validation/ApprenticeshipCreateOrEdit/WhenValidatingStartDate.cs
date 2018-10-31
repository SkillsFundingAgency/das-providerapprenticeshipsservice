using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
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
            MockMediator.Verify(x => x.Send(It.IsAny<GetTrainingProgrammesQueryRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
