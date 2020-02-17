using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.ViewModels
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
        }
        
        [TestFixture]
        public class WhenCallingToQueryString
        {
            private const string DefaultQueryString = "ResetFilter=True&PageNumber=1";

            [Test]
            public void AndNoValuesThenReturnsReset()
            {
                var sut = new ApprenticeshipFiltersViewModel();

                sut.ToQueryString().Should().Be(DefaultQueryString);
            }

            [Test, AutoData]
            public void AndPageNumberNotDefaultThenSetInQueryString(int pageNumber)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    PageNumber = pageNumber
                };

                sut.ToQueryString().Should().Contain($"PageNumber={pageNumber}");
            }

            [Test]
            public void AndResetFilterNotDefaultThenSetInQueryString()
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    ResetFilter = true
                };

                sut.ToQueryString().Should().Contain($"ResetFilter={true}");
            }

            [Test, AutoData]
            public void AndFilterListsHaveValuesThenReturnsValues(
                ApprenticeshipFiltersViewModel sut)
            {
                var expected = TestHelper.Clone(sut);
                var actual = sut.ToQueryString();


                actual.Should().Contain($"{string.Join("&", expected.Status.Select(s => $"Status={s}"))}");
                actual.Should().Contain($"{string.Join("&", expected.FundingStatus.Select(s => $"FundingStatus={s}"))}");
                actual.Should().Contain($"{string.Join("&", expected.RecordStatus.Select(s => $"RecordStatus={s}"))}");
                actual.Should().Contain($"{string.Join("&", expected.Course.Select(s => $"Course={s}"))}");
                actual.Should().Contain($"{string.Join("&", expected.Employer.Select(s => $"Employer={s}"))}");
            }

            [Test, AutoData]
            public void AndOptionsHasValuesThenIgnores(
                List<KeyValuePair<string, string>> apprenticeshipStatusOptions,
                List<KeyValuePair<string, string>> fundingStatusOptions,
                List<KeyValuePair<string, string>> employerOrganisationOptions,
                List<KeyValuePair<string, string>> recordStatusOptions,
                List<KeyValuePair<string, string>> trainingCourseOptions)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    ApprenticeshipStatusOptions = apprenticeshipStatusOptions,
                    FundingStatusOptions = fundingStatusOptions,
                    EmployerOrganisationOptions = employerOrganisationOptions,
                    RecordStatusOptions = recordStatusOptions,
                    TrainingCourseOptions = trainingCourseOptions
                };

                sut.ToQueryString().Should().Be(DefaultQueryString);
            }
        }
    }
}