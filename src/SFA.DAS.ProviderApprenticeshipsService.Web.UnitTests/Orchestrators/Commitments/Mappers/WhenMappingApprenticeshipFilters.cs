using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeshipFilters
    {
        private ApprenticeshipFiltersMapper _mapper;
        private Facets _facets;


        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipFiltersMapper();

            _facets = new Facets
            {
                ApprenticeshipStatuses = new List<FacetItem<ApprenticeshipStatus>>
                {
                    new FacetItem<ApprenticeshipStatus> {Data = ApprenticeshipStatus.WaitingToStart},
                    new FacetItem<ApprenticeshipStatus> {Data = ApprenticeshipStatus.Paused},
                    new FacetItem<ApprenticeshipStatus> {Data = ApprenticeshipStatus.Stopped, Selected = true},
                    new FacetItem<ApprenticeshipStatus> {Data = ApprenticeshipStatus.Paused},
                    new FacetItem<ApprenticeshipStatus> {Data = ApprenticeshipStatus.Finished},
                    new FacetItem<ApprenticeshipStatus> {Data = ApprenticeshipStatus.Live}
                },
                EmployerOrganisations = new List<FacetItem<EmployerOrganisation>>
                {
                    new FacetItem<EmployerOrganisation> {Data = new EmployerOrganisation {Id = 1, Name = "Employer 1"}, Selected = true },
                    new FacetItem<EmployerOrganisation> {Data = new EmployerOrganisation {Id = 2, Name = "Employer 2"}},
                    new FacetItem<EmployerOrganisation> {Data = new EmployerOrganisation {Id = 3, Name = "Employer 3"}, Selected = true },
                },
                RecordStatuses = new List<FacetItem<RecordStatus>>
                {
                    new FacetItem<RecordStatus> {Data = RecordStatus.ChangeRequested, Selected = true },
                    new FacetItem<RecordStatus> {Data = RecordStatus.ChangesForReview, Selected = false} ,
                    new FacetItem<RecordStatus> {Data = RecordStatus.ChangesPending, Selected = false },
                    new FacetItem<RecordStatus> {Data = RecordStatus.NoActionNeeded, Selected = true },
                },
                TrainingCourses = new List<FacetItem<TrainingCourse>>
                {
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse {Id = "C1", Name = "Course 1", TrainingType = TrainingType.Framework},
                        Selected = true
                    },
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse {Id = "C2", Name = "Course 2", TrainingType = TrainingType.Framework}
                    },
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse {Id = "C3", Name = "Course 3", TrainingType = TrainingType.Standard},
                        Selected = true
                    },
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse {Id = "C4", Name = "Course 4", TrainingType = TrainingType.Framework}
                    },
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse {Id = "C5", Name = "Course 5", TrainingType = TrainingType.Framework}
                    }
                }
            };
        }

        [Test]
        public void ThenApprenticeshipStatusesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.Map(_facets);

            //Assert
            Assert.AreEqual(6, result.ApprenticeshipStatusOptions.Count);

            var actual = result.ApprenticeshipStatusOptions.First();
            Assert.AreEqual("WaitingToStart", actual.Key);
            Assert.AreEqual((Domain.ApprenticeshipStatus.WaitingToStart).GetDescription(), actual.Value);

            Assert.AreEqual(1, result.Status.Count);
        }

        [Test]
        public void ThenEmployerOrganisationsAreMappedCorrectly()
        {
            //Act
            var result = _mapper.Map(_facets);

            //Assert
            Assert.AreEqual(3, result.EmployerOrganisationOptions.Count);

            var actual = result.EmployerOrganisationOptions.First();
            Assert.AreEqual("1", actual.Key);
            Assert.AreEqual("Employer 1", actual.Value);

            Assert.AreEqual(2, result.Employer.Count);
        }

        [Test]
        public void ThenRecordStatusesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.Map(_facets);

            //Assert
            Assert.AreEqual(4, result.RecordStatusOptions.Count);

            var actual = result.RecordStatusOptions.First();
            Assert.AreEqual("ChangeRequested", actual.Key);
            Assert.AreEqual((Domain.RecordStatus.ChangeRequested).GetDescription(), actual.Value);

            Assert.AreEqual(2, result.RecordStatus.Count);
        }

        [Test]
        public void ThenTrainingCoursesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.Map(_facets);

            //Assert
            Assert.AreEqual(5, result.TrainingCourseOptions.Count);

            var actual = result.TrainingCourseOptions.First();
            Assert.AreEqual("C1", actual.Key);
            Assert.AreEqual("Course 1", actual.Value);

            Assert.AreEqual(2, result.Course.Count);
        }

    }
}
