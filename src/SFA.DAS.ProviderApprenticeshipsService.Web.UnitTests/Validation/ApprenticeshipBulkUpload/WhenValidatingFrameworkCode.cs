﻿using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingFrameworkCode : ApprenticeshipBulkUploadValidationTestBase
    {
        [Test]
        public void AndNoFrameworkCodeAndIsTransferThenValid()
        {
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(DateTime.Parse("2018-05-01"));
            ValidModel.ApprenticeshipViewModel.IsPaidForByTransfer = true;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public void AndHasFrameworkCodeAndIsTransferThenInvalid()
        {
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(DateTime.Parse("2018-05-01"));
            ValidModel.CsvRecord.ProgType = "2";
            ValidModel.CsvRecord.StdCode = null;
            ValidModel.CsvRecord.PwayCode = "45";
            ValidModel.CsvRecord.FworkCode = "34";
            ValidModel.ApprenticeshipViewModel.IsPaidForByTransfer = true;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().Be("Entered apprenticeship type is a framework.<br/>All apprenticeship types must be apprenticeship standards.");
        }

        [Test]
        public void AndHasFrameworkCodeAndIsNotTransferThenValid()
        {
            ValidModel.CsvRecord.ProgType = "2";
            ValidModel.CsvRecord.StdCode = null;
            ValidModel.CsvRecord.PwayCode = "45";
            ValidModel.CsvRecord.FworkCode = "34";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}