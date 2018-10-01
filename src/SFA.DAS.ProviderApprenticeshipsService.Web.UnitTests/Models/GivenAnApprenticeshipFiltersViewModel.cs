using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Models
{
    [TestFixture]
    public class GivenAnApprenticeshipFiltersViewModel
    {
        [TestFixture]
        public class WhenCallingHasValues
        {
            [Test]
            public void AndNoValuesThenReturnsFalse()
            {
                var sut = new ApprenticeshipFiltersViewModel();

                sut.HasValues().Should().BeFalse();
            }

            [Test, AutoData]
            public void AndStatusHasValuesThenReturnsTrue(
                List<string> statuses)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    Status = statuses
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndRecordStatusHasValuesThenReturnsTrue(
                List<string> statuses)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    RecordStatus = statuses
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndEmployerHasValuesThenReturnsTrue(
                List<string> employers)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    Employer = employers
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndCourseHasValuesThenReturnsTrue(
                List<string> courses)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    Course = courses
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndFundingStatusHasValuesThenReturnsTrue(
                List<string> statuses)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    FundingStatus = statuses
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndSearchInputHasValueThenReturnsTrue(
                string searchInput)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    SearchInput = searchInput
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndPageNumberHasValueThenReturnsTrue(
                Generator<int> intGenerator)
            {
                var pageNumber = intGenerator.Single(i => i > 1);

                var sut = new ApprenticeshipFiltersViewModel
                {
                    PageNumber = pageNumber
                };

                sut.HasValues().Should().BeTrue();
            }
        }
    }
}