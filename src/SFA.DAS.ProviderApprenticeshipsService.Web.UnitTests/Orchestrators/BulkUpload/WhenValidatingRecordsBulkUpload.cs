using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

using BulkUploadValidator = SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload.BulkUploadValidator;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{

    [TestFixture]
    public class WhenValidatingRecordsBulkUpload
    {
        BulkUploadValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new BulkUploadValidator(new ProviderApprenticeshipsServiceConfiguration(), Mock.Of<ILog>());
        }

        [Test]
        public void EverythingIsValidating()
        {
            var errors = _sut.ValidateFields(GetTestData(), TrainingProgrammes(), "ABBA123");
            errors.Count().Should().Be(0);
        }

        [Test]
        public void NoRecords()
        {
            var errors = _sut.ValidateFields(
                new ApprenticeshipUploadModel[0], 
                TrainingProgrammes(), "ABBA123").ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().ShouldBeEquivalentTo("File contains no records");
        }

        [Test]
        public void MissingTraingCode()
        {
            var errors = _sut.ValidateFields(GetTestData(), new List<ITrainingProgramme>(), "ABBA123").ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().ShouldBeEquivalentTo("Row:1 - Not a valid <strong>Training code</strong>");
        }

        [Test]
        public void FailingValidationOnName()
        {
            var errors = _sut.ValidateFields(GetFailingTestData(), TrainingProgrammes(), "ABBA123").ToList();
            errors.Count.Should().Be(4);
            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().Contain("Row:1 - You must enter <strong>Given names</strong> that are no longer than 100 characters");
            messages.Should().Contain("Row:1 - You must enter a <strong>Family name</strong> that's no longer than 100 characters");
            messages.Should().Contain("Row:2 - You must enter <strong>Given names</strong> that are no longer than 100 characters");
            messages.Should().Contain("Row:2 - You must enter a <strong>Family name</strong> that's no longer than 100 characters");
        }

        [Test]
        public void FailingValidationCohortRef()
        {
            var testData = GetFailingTestData().ToList();
            var first = testData[0];
            var second = testData[1];
            first.CsvRecord.CohortRef = "Abba123";
            second.CsvRecord.CohortRef = "Other reference";

            var errors = _sut.ValidateFields(new List<ApprenticeshipUploadModel> { first, second } , TrainingProgrammes(), "ABBA123").ToList();
            errors.Count.Should().Be(6);
            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().Contain("The Cohort Reference must be the same for all learners in the file");
            messages.Should().Contain("The Cohort Reference does not match the current cohort");
        }

        [Test]
        public void FailingValidationCohortRefNotCurrentRef()
        {
            var testData = GetFailingTestData().ToList();
            var first = testData[0];
            var second = testData[1];
            first.CsvRecord.CohortRef = "Other ref";
            second.CsvRecord.CohortRef = "Other ref";

            var errors = _sut.ValidateFields(new List<ApprenticeshipUploadModel> { first, second }, TrainingProgrammes(), "ABBA123").ToList();
            errors.Count.Should().Be(5);
            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().NotContain("The Cohort Reference must be the same for all learners in the file");
            messages.Should().Contain("The Cohort Reference does not match the current cohort");
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

        private IEnumerable<ApprenticeshipUploadModel> GetTestData()
        {
            var apprenticeships = new List<ApprenticeshipViewModel>
            {
                new ApprenticeshipViewModel
                {
                    FirstName = "Bob", LastName = "The cat", DateOfBirth = new DateTimeViewModel(8, 12, 1998), TrainingCode = "2", ULN = "1234567890", ProgType = 25,
                    StartDate = new DateTimeViewModel(null, 8, 2120), EndDate = new DateTimeViewModel(null, 12, 2125), Cost = "15000", EmployerRef = "Abba123"
                }
            };
            var records = new List<CsvRecord>
                              {
                                  new CsvRecord { ProgType = 23, FworkCode = 18, PwayCode = 26, CohortRef = "ABBA123" }
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
                    FirstName = new string('*', 101), LastName = "", DateOfBirth = new DateTimeViewModel(8, 12, 1998), TrainingCode = "2", ULN = "1234567890", ProgType = 25,
                    StartDate = new DateTimeViewModel(null, 8, 2120), EndDate = new DateTimeViewModel(null, 12, 2125), Cost = "15000", EmployerRef = "Abba123"
                }
            };

            var records = new List<CsvRecord>
            {
                new CsvRecord { ProgType = 25, StdCode = 2, CohortRef = "ABBA123" },
                new CsvRecord { ProgType = 25, StdCode = 2, CohortRef = "ABBA123" }
            };

            return apprenticeships.Zip(
                records,
                (a, r) => new ApprenticeshipUploadModel { ApprenticeshipViewModel = a, CsvRecord = r });
        }
    }
}
