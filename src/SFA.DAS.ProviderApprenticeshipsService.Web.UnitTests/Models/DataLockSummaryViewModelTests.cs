using System.Collections.Generic;

using FluentAssertions;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Models
{
    [TestFixture]
    public class DataLockSummaryViewModelTests
    {
        private DataLockSummaryViewModel _model;

        [SetUp]
        public void SetUp()
        {
            var dataLockWithCourseMismatch = new List<DataLockViewModel>();
            var dataLockWithOnlyPriceMismatch = new List<DataLockViewModel>();
            _model = new DataLockSummaryViewModel
                         {
                             DataLockWithCourseMismatch = dataLockWithCourseMismatch,
                             DataLockWithOnlyPriceMismatch = dataLockWithOnlyPriceMismatch,
                             ShowCourseDataLockTriageLink = true,
                             ShowPriceDataLockTriageLink = true
                         };
        }

        [Test]
        public void ShouldShowNoTitleIfNoDataLock()
        {
            _model.ShowCourseDataLockTriageLink = false;
            _model.ShowPriceDataLockTriageLink = false;

            _model.DataLockSummaryTitle.Should().Be("");
        }

        [Test]
        public void ShouldShowPriceAndCourse()
        {
            _model.DataLockWithCourseMismatch.Add(new DataLockViewModel());
            _model.DataLockWithOnlyPriceMismatch.Add(new DataLockViewModel());

            _model.DataLockSummaryTitle.Should().Be("Price and course mismatch");
        }

        [Test]
        public void ShouldShowPrice()
        {
            _model.DataLockWithOnlyPriceMismatch.Add(new DataLockViewModel());

            _model.DataLockSummaryTitle.Should().Be("Price mismatch");
        }

        [Test]
        public void ShouldShowCourse()
        {
            _model.DataLockWithCourseMismatch.Add(new DataLockViewModel());

            _model.DataLockSummaryTitle.Should().Be("Course mismatch");
        }

        [Test]
        public void ShouldShowPriceAndCourseInOne()
        {
            _model.DataLockWithCourseMismatch.Add(new DataLockViewModel {DataLockErrorCode = (DataLockErrorCode) 68 });

            _model.DataLockSummaryTitle.Should().Be("Price and course mismatch");
        }
    }
}
