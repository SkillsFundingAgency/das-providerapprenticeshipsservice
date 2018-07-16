using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeshipSearchQuery
    {
        private ApprenticeshipFiltersMapper _mapper;
        private ApprenticeshipFiltersViewModel _filtersViewModel;

        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipFiltersMapper();

            _filtersViewModel = new ApprenticeshipFiltersViewModel
            {
                PageNumber = 18,
                Status = new List<string>
                {
                    ApprenticeshipStatus.Live.ToString(),
                    ApprenticeshipStatus.Paused.ToString()
                },
                RecordStatus = new List<string>
                {
                    RecordStatus.NoActionNeeded.ToString(),
                    RecordStatus.ChangesForReview.ToString(),
                    RecordStatus.ChangeRequested.ToString()
                },
                Employer = new List<string>
                {
                    "12345"
                },
                Course = new List<string>
                {
                    "CourseId1", "CourseId2", "CourseId3", "CourseId4"
                },
                FundingStatus = new List<string>
                {
                    FundingStatus.TransferFunded.ToString()
                }
            };
        }

        [Test]
        public void ThenApprenticeshipStatusesAreMappedCorrectly()
        {
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            CollectionAssert.AreEqual(
                new List<ApprenticeshipStatus> { ApprenticeshipStatus.Live, ApprenticeshipStatus.Paused }, 
                result.ApprenticeshipStatuses);
        }

        [Test]
        public void ThenRecordStatusesAreMappedCorrectly()
        {
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            CollectionAssert.AreEqual(
                new List<RecordStatus> { RecordStatus.NoActionNeeded, RecordStatus.ChangesForReview, RecordStatus.ChangeRequested },
                result.RecordStatuses);
        }

        [Test]
        public void ThenEmployersAreMappedCorrectly()
        {
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            CollectionAssert.AreEqual(new List<string> { "12345" }, result.EmployerOrganisationIds);
        }

        [Test]
        public void ThenCoursesAreMappedCorrectly()
        {
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            CollectionAssert.AreEqual(new List<string> { "CourseId1", "CourseId2", "CourseId3", "CourseId4" }, result.TrainingCourses);
        }

        [Test]
        public void ThenFundingStatusesAreMappedCorrectly()
        {
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            CollectionAssert.AreEqual(new List<FundingStatus> { FundingStatus.TransferFunded }, result.FundingStatuses);
        }

        [Test]
        public void ThenPageNumberIsMappedCorrectly()
        {
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            Assert.AreEqual(18, result.PageNumber);
        }
    }
}
