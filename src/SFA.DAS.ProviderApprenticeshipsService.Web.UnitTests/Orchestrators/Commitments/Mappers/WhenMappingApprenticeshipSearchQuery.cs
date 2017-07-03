using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture()]
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
                }
            };
        }

        [Test]
        public void ThenApprenticeshipStatusesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(2, result.ApprenticeshipStatuses.Count);
            Assert.AreEqual((int)ApprenticeshipStatus.Live, (int)result.ApprenticeshipStatuses[0]);
        }

        [Test]
        public void ThenRecordStatusesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(3, result.RecordStatuses.Count);
            Assert.AreEqual((int)RecordStatus.NoActionNeeded, (int)result.RecordStatuses[0]);
        }

        [Test]
        public void ThenEmployersAreMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(1, result.EmployerOrganisationIds.Count);
            Assert.AreEqual("12345", result.EmployerOrganisationIds[0]);
        }

        [Test]
        public void ThenCoursesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(4, result.TrainingCourses.Count);
            Assert.AreEqual("CourseId1", result.TrainingCourses[0]);
        }

        [Test]
        public void ThenPageNumberIsMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(18, result.PageNumber);
        }
    }
}
