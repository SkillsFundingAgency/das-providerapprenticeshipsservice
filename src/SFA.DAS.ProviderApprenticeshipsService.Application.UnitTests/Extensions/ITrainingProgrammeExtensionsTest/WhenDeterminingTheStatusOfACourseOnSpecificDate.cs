﻿using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Extensions.ITrainingProgrammeExtensionsTest;

[TestFixture]
public class WhenDeterminingTheStatusOfACourseOnSpecificDate
{
    [TestCase("2016-01-01", "2016-12-01", "2016-06-01", TrainingProgrammeStatus.Active, Description = "Within date range")]
    [TestCase("2016-01-01", "2016-12-01", "2016-01-01", TrainingProgrammeStatus.Active, Description = "Within date range (on start day)")]
    [TestCase("2016-01-01", "2016-12-01", "2016-12-01", TrainingProgrammeStatus.Active, Description = "Within date range (on last day)")]
    [TestCase("2016-01-15", "2016-12-15", "2016-01-01", TrainingProgrammeStatus.Active, Description = "Within date range - ignoring start day")]
    //[TestCase("2016-01-15", "2016-12-15", "2016-12-30", TrainingProgrammeStatus.Active, Description = "Within date range - ignoring end day")]
    [TestCase("2016-01-15", "2016-12-15", "2016-12-30", TrainingProgrammeStatus.Expired, Description = "Past date range (but in same month as courseEnd")]
    [TestCase(null, "2016-12-01", "2016-06-01", TrainingProgrammeStatus.Active, Description = "Within date range with no defined course start date")]
    [TestCase("2016-01-01", null, "2016-06-01", TrainingProgrammeStatus.Active, Description = "Withing date range, with no defined course end date")]
    [TestCase(null, null, "2016-06-01", TrainingProgrammeStatus.Active, Description = "Within date range, with no defined course effective dates")]
    [TestCase("2016-01-01", "2016-12-01", "2015-06-01", TrainingProgrammeStatus.Pending, Description = "Outside (before) date range")]
    [TestCase("2016-01-01", "2016-12-01", "2015-12-31", TrainingProgrammeStatus.Pending, Description = "Outside (immediately before) date range")]
    [TestCase("2016-01-01", "2016-12-01", "2017-06-01", TrainingProgrammeStatus.Expired, Description = "Outside (after) date range")]
    [TestCase("2016-01-01", "2016-12-01", "2017-01-01", TrainingProgrammeStatus.Expired, Description = "Outside (immediately after) date range")]
    [TestCase(null, "2016-12-01", "2017-06-01", TrainingProgrammeStatus.Expired, Description = "Outside (after) date range with no defined course start date")]
    public void ThenTheCourseEffectiveDatesAreUsedToDetermineTheStatus(DateTime? courseStart, DateTime? courseEnd, DateTime effectiveDate, TrainingProgrammeStatus expectStatus)
    {
        //Arrange
        var course = new TrainingProgramme
        {
            EffectiveFrom = courseStart, 
            EffectiveTo = courseEnd
        };

        //Act
        var result = course.GetStatusOn(effectiveDate);

        //Assert
        Assert.That(expectStatus, Is.EqualTo(result));
    }
}