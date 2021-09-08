using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
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
            errors.FirstOrDefault().ToString().Should().BeEquivalentTo("No apprentice details found. Please check your file and upload again.");
        }

        [Test]
        public void AndMissingCourseCodeThenFailsValidation()
        {
            var errors = _sut.ValidateRecords(GetTestData(), new List<TrainingProgramme>()).ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().Should().BeEquivalentTo("Row:1 - Not a valid <strong>Training code</strong>");
        }

        [Test]
        public void AndMissingStartDateThenFailsValidation()
        {
            var testData = GetTestData();
            testData.First().ApprenticeshipViewModel.StartDate = null;

            var errors = _sut.ValidateRecords(testData, TrainingProgrammes()).ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().Should().BeEquivalentTo("Row:1 - You must enter the <strong>start date</strong>, for example 2017-09-01");
        }

        [TestCase(2018, 12, "This training course is only available to apprentices with a start date after 05 2019", Description = "Start date before (month numerically higher)")]
        [TestCase(2019, 04, "This training course is only available to apprentices with a start date after 05 2019", Description = "Start date before")]
        [TestCase(2019, 05, "This training course is only available to apprentices with a start date after 05 2019", Description = "Start date just before")]
        [TestCase(2020, 10, "This training course is only available to apprentices with a start date before 10 2020", Description = "Start date just after")]
        [TestCase(2020, 11, "This training course is only available to apprentices with a start date before 10 2020", Description = "Start date after")]
        [TestCase(2021, 01, "This training course is only available to apprentices with a start date before 10 2020", Description = "Start date after (month numerically lower)")]
        public void AndCourseCodeIsPendingOrExpiredThenValidationFails(int startYear, int startMonth, string expectedErrorMessage)
        {
            var errors = _sut.ValidateRecords(GetTestData(startYear, startMonth, 2021, 09), new List<TrainingProgramme>
            {
                new TrainingProgramme
                {
                    EffectiveFrom = new DateTime(2019,6,1),
                    EffectiveTo = new DateTime(2020,9,1),
                    CourseCode = "2"
                }
            }).ToList();

            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().Should().BeEquivalentTo($"Row:1 - {expectedErrorMessage}");
        }

        [TestCase(2019, 06, Description = "Start date first valid")]
        [TestCase(2019, 07, Description = "Start date valid")]
        [TestCase(2019, 09, Description = "Start date last valid")]
        public void AndCourseCodeIsActiveThenValidationPasses(int startYear, int startMonth)
        {
            var errors = _sut.ValidateRecords(GetTestData(startYear, startMonth, 2021, 09), new List<TrainingProgramme>
            {
                new TrainingProgramme
                {
                    EffectiveFrom = new DateTime(2019,6,1),
                    EffectiveTo = new DateTime(2020,9,1),
                    CourseCode = "2"
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
            errors.Count.Should().Be(1);
            var messages = errors.Select(m => m.ToString()).ToList();            
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

        [Test]
        public void FailingValidationCohortNotUniqueEmails()
        {
            var testData = GetFailingTestData().ToList();
            var first = testData[0];
            var second = testData[1];
            first.ApprenticeshipViewModel.EmailAddress = "apprentice@test.com";
            second.ApprenticeshipViewModel.EmailAddress = "apprentice@test.com";

            var errors = _sut.ValidateEmailUniqueness(new List<ApprenticeshipUploadModel> { first, second }).ToList();

            var messages = errors.Select(m => m.ToString()).ToList();            
            messages.Should().Contain("The email address has already been used for an apprentice in this cohort");
        }

        [Test]
        public void FailingValidationOnAgreementId()
        {
            var testData = GetFailingTestData().ToList();
            var first = testData[0];
            var second = testData[1];          

            var errors = _sut.ValidateAgreementId(new List<ApprenticeshipUploadModel> { first, second }, "XYZUV").ToList();

            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().Contain("The employer on the cohort does not match the Agreement ID");
        }

        private List<TrainingProgramme> TrainingProgrammes()
        {
            return new List<TrainingProgramme>
                       {   
                            new TrainingProgramme { Name = "Standard 1", CourseCode = "1" },
                            new TrainingProgramme { Name = "Standard 2", CourseCode = "2" }
                       };
        }

        private IEnumerable<ApprenticeshipUploadModel> GetTestData(int startYear = 2020, int startMonth = 8, int endYear = 2125, int endMonth = 12)
        {
            var apprenticeships = new List<ApprenticeshipViewModel>
            {
                new ApprenticeshipViewModel
                {
                    FirstName = "Bob", LastName = "The cat", DateOfBirth = new DateTimeViewModel(8, 12, 1998), CourseCode = "2", ULN = "1234567890",                     
                    StartDate = new DateTimeViewModel(01, startMonth, startYear), EndDate = new DateTimeViewModel(null, endMonth, endYear), Cost = "15000", EmployerRef = "Abba123",
                    EmailAddress = "apprentice1@test.com", AgreementId = "XYZUR"
                }
            };
            var records = new List<CsvRecord>
                              {
                                  new CsvRecord { StdCode = "12345",  CohortRef = "ABBA123", EmailAddress = "apprentice1@test.com", AgreementId = "XYZUR" }
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
                    FirstName = " ", LastName = new string('*', 101), DateOfBirth = new DateTimeViewModel(8, 12, 1998), CourseCode = "2", ULN = "1234567890", 
                    StartDate = new DateTimeViewModel(null, 8, 2019), EndDate = new DateTimeViewModel(null, 12, 2019), Cost = "15000", EmployerRef = "Ab123",
                    EmailAddress = "apprentice1@test.com", AgreementId="XYZUR"
                },
                new ApprenticeshipViewModel
                {
                    FirstName = new string('*', 101), LastName = "", DateOfBirth = new DateTimeViewModel(8, 12, 1998), CourseCode = "2", ULN = "1234567891",
                    StartDate = new DateTimeViewModel(null, 8, 2019), EndDate = new DateTimeViewModel(null, 12, 2019), Cost = "15000", EmployerRef = "Abba123",
                    EmailAddress = "apprentice2@test.com", AgreementId="XYZUR"
                }
            };

            var records = new List<CsvRecord>
            {
                new CsvRecord { StdCode = "2", CohortRef = "ABBA123", EmailAddress = "apprentice1@test.com", AgreementId="XYZUR" },                
                new CsvRecord { StdCode = "2", CohortRef = "ABBA123", EmailAddress = "apprentice2@test.com", AgreementId="XYZUR" }
            };

            return apprenticeships.Zip(
                records,
                (a, r) => new ApprenticeshipUploadModel { ApprenticeshipViewModel = a, CsvRecord = r });
        }
    }
}
