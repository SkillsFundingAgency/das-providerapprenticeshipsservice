using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

using BulkUploadValidator = SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload.BulkUploadValidator;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{

    [TestFixture]
    public class WhenValidatingRecordsBulkUpload
    {
        private BulkUploadValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = BulkUploadTestHelper.GetBulkUploadValidator();
        }

        [Test]
        public void EverythingIsValid()
        {
            var errors = _sut.ValidateRecords(GetTestData(), TrainingProgrammes());
            errors.Count().Should().Be(0);
        }

        [Test]
        public void NoRecords()
        {
            var errors = _sut.ValidateCohortReference(new ApprenticeshipUploadModel[0], "ABBA123").ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().ShouldBeEquivalentTo("No apprentice details found. Please check your file and upload again.");
        }

        [Test]
        public void AndMissingTrainingCodeThenFailsValidation()
        {
            var errors = _sut.ValidateRecords(GetTestData(), new List<ITrainingProgramme>()).ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().ShouldBeEquivalentTo("Row:1 - Not a valid <strong>Training code</strong>");
        }

        [Test]
        public void AndMissingStartDateThenFailsValidation()
        {
            var testData = GetTestData();
            testData.First().ApprenticeshipViewModel.StartDate = null;

            var errors = _sut.ValidateRecords(testData, TrainingProgrammes()).ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().ShouldBeEquivalentTo("Row:1 - You must enter the <strong>start date</strong>, for example 2017-09");
        }

        [TestCase(2018, 12, "This training course is only available to apprentices with a start date after 05 2019", Description = "Start date before (month numerically higher)")]
        [TestCase(2019, 04, "This training course is only available to apprentices with a start date after 05 2019", Description = "Start date before")]
        [TestCase(2019, 05, "This training course is only available to apprentices with a start date after 05 2019", Description = "Start date just before")]
        [TestCase(2020, 10, "This training course is only available to apprentices with a start date before 10 2020", Description = "Start date just after")]
        [TestCase(2020, 11, "This training course is only available to apprentices with a start date before 10 2020", Description = "Start date after")]
        [TestCase(2021, 01, "This training course is only available to apprentices with a start date before 10 2020", Description = "Start date after (month numerically lower)")]
        public void AndTrainingCodeIsPendingOrExpiredThenValidationFails(int startYear, int startMonth, string expectedErrorMessage)
        {
            var errors = _sut.ValidateRecords(GetTestData(startYear, startMonth, 2021, 09), new List<ITrainingProgramme>
            {
                new Standard
                {
                    EffectiveFrom = new DateTime(2019,6,1),
                    EffectiveTo = new DateTime(2020,9,1),
                    Id = "2"
                }
            }).ToList();

            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().ShouldBeEquivalentTo($"Row:1 - {expectedErrorMessage}");
        }

        [TestCase(2019, 06, Description = "Start date first valid")]
        [TestCase(2019, 07, Description = "Start date valid")]
        [TestCase(2019, 09, Description = "Start date last valid")]
        public void AndTrainingCodeIsActiveThenValidationPasses(int startYear, int startMonth)
        {
            var errors = _sut.ValidateRecords(GetTestData(startYear, startMonth, 2021, 09), new List<ITrainingProgramme>
            {
                new Standard
                {
                    EffectiveFrom = new DateTime(2019,6,1),
                    EffectiveTo = new DateTime(2020,9,1),
                    Id = "2"
                }
            }).ToList();

            errors.Count.Should().Be(0);
        }

        [Test]
        public void FailingValidationOnName()
        {
            var errors = _sut.ValidateRecords(GetFailingTestData(), TrainingProgrammes()).ToList();
            errors.Count.Should().Be(4);
            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().Contain("Row:1 - <strong>First name</strong> must be entered");
            messages.Should().Contain("Row:1 - You must enter a <strong>last name</strong> that's no longer than 100 characters");
            messages.Should().Contain("Row:2 - You must enter a <strong>first name</strong> that's no longer than 100 characters");
            messages.Should().Contain("Row:2 - <strong>Last name</strong> must be entered");
        }

        [Test]
        public void FailingValidationCohortRef()
        {
            var testData = GetFailingTestData().ToList();
            var first = testData[0];
            var second = testData[1];
            first.CsvRecord.CohortRef = "Abba123";
            second.CsvRecord.CohortRef = "Other reference";

            var errors = _sut.ValidateCohortReference(new List<ApprenticeshipUploadModel> { first, second }, "ABBA123").ToList();
            errors.Count.Should().Be(2);
            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().Contain("The cohort reference must be the same for all apprentices in your upload file");
            messages.Should().Contain("The cohort reference does not match your current cohort");
        }

        [Test]
        public void FailingValidationCohortRefNotCurrentRef()
        {
            var testData = GetFailingTestData().ToList();
            var first = testData[0];
            var second = testData[1];
            first.CsvRecord.CohortRef = "Other ref";
            second.CsvRecord.CohortRef = "Other ref";

            var errors = _sut.ValidateCohortReference(new List<ApprenticeshipUploadModel> { first, second }, "ABBA123").ToList();

            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().NotContain("The cohort reference must be the same for all apprentices in your upload file");
            messages.Should().Contain("The cohort reference does not match your current cohort");
        }

        [Test]
        public void FailingValidationCohortNotUniqueUlns()
        {
            var testData = GetFailingTestData().ToList();
            var first = testData[0];
            var second = testData[1];
            first.ApprenticeshipViewModel.ULN = "1112220001";
            second.ApprenticeshipViewModel.ULN = "1112220001";

            var errors = _sut.ValidateCohortReference(new List<ApprenticeshipUploadModel> { first, second }, "ABBA123").ToList();

            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().NotContain("The cohort reference must be the same for all apprentices in your upload file");
            messages.Should().Contain("The unique learner number must be unique within the cohort");
        }

        private List<ITrainingProgramme> TrainingProgrammes()
        {
            return new List<ITrainingProgramme>
                       {
                            new Framework { FrameworkName = "Framework1", Id = "1-2-3"},
                            new Framework { FrameworkName = "Framework2", Id = "`4-5-6" },
                            new Standard { Title = "Standard 1", Id = "1" },
                            new Standard { Title = "Standard 2", Id = "2" }
                       };
        }

        private IEnumerable<ApprenticeshipUploadModel> GetTestData(int startYear = 2120, int startMonth = 8, int endYear = 2125, int endMonth = 12)
        {
            var apprenticeships = new List<ApprenticeshipViewModel>
            {
                new ApprenticeshipViewModel
                {
                    FirstName = "Bob", LastName = "The cat", DateOfBirth = new DateTimeViewModel(8, 12, 1998), TrainingCode = "2", ULN = "1234567890", ProgType = 25,
                    StartDate = new DateTimeViewModel(null, startMonth, startYear), EndDate = new DateTimeViewModel(null, endMonth, endYear), Cost = "15000", EmployerRef = "Abba123"
                }
            };
            var records = new List<CsvRecord>
                              {
                                  new CsvRecord { ProgType = "23", FworkCode = "18", PwayCode = "26", CohortRef = "ABBA123" }
                              };
            return apprenticeships.Zip(
                records,
                (a, r) => new ApprenticeshipUploadModel { ApprenticeshipViewModel = a, CsvRecord = r });
        }

        private IEnumerable<ApprenticeshipUploadModel> GetFailingTestData()
        {
            var apprenticeships = new List<ApprenticeshipViewModel>
            {
                new ApprenticeshipViewModel
                {
                    FirstName = " ", LastName = new string('*', 101), DateOfBirth = new DateTimeViewModel(8, 12, 1998), TrainingCode = "2", ULN = "1234567890", ProgType = 25,
                    StartDate = new DateTimeViewModel(null, 8, 2120), EndDate = new DateTimeViewModel(null, 12, 2125), Cost = "15000", EmployerRef = "Ab123"
                },
                new ApprenticeshipViewModel
                {
                    FirstName = new string('*', 101), LastName = "", DateOfBirth = new DateTimeViewModel(8, 12, 1998), TrainingCode = "2", ULN = "1234567891", ProgType = 25,
                    StartDate = new DateTimeViewModel(null, 8, 2120), EndDate = new DateTimeViewModel(null, 12, 2125), Cost = "15000", EmployerRef = "Abba123"
                }
            };

            var records = new List<CsvRecord>
            {
                new CsvRecord { ProgType = "25", StdCode = "2", CohortRef = "ABBA123" },
                new CsvRecord { ProgType = "25", StdCode = "2", CohortRef = "ABBA123" }
            };

            return apprenticeships.Zip(
                records,
                (a, r) => new ApprenticeshipUploadModel { ApprenticeshipViewModel = a, CsvRecord = r });
        }
    }
}
