﻿using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingTrainingCode : ApprenticeshipValidationTestBase
    {
        [Test]
        public void ShouldBeValidIfNoTrainingCodeValuesSet()
        {
            ValidModel.TrainingCode = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldBeValidIfTrainingCodeValuesSet()
        {
            ValidModel.TrainingCode = "123";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}
