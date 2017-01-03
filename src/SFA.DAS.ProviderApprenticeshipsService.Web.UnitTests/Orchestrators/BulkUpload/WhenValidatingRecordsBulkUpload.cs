using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{

    [TestFixture]
    public class WhenValidatingRecordsBulkUpload
    {
        BulkUploader _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new BulkUploader();
        }

        [Test]
        public void EverythingIsValidating()
        {
            var errors = _sut.ValidateFields(GetTestData(), TrainingProgrammes());
            errors.Count().Should().Be(0);
        }

        [Test]
        public void NoRecords()
        {
            var errors = _sut.ValidateFields(
                new ApprenticeshipUploadModel[0], 
                TrainingProgrammes()).ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().ShouldBeEquivalentTo("File contains no records");
        }

        [Test]
        public void MissingTraingCode()
        {
            var errors = _sut.ValidateFields(GetTestData(), new List<ITrainingProgramme>()).ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ToString().ShouldBeEquivalentTo("Row:2 - Not a valid training code");
        }

        [Test]
        public void FailingValidationOnName()
        {
            var errors = _sut.ValidateFields(GetFailingTestData(), TrainingProgrammes()).ToList();
            errors.Count.Should().Be(4);
            var messages = errors.Select(m => m.ToString()).ToList();
            messages.Should().Contain("Row:2 - The Given names must be entered");
            messages.Should().Contain("Row:2 - The Given names must be entered and must not be more than 100 characters in length");
            messages.Should().Contain("Row:3 - The Family name must be entered and must not be more than 100 characters in length");
            messages.Should().Contain("Row:3 - The Given names must be entered");
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
                    StartDate = new DateTimeViewModel(null, 8, 2120), EndDate = new DateTimeViewModel(null, 12, 2125), Cost = "15000", NINumber = "SE345678A", EmployerRef = "Abba123"
                }
            };
            var records = new List<CsvRecord>
                              {
                                  new CsvRecord { ProgType = 23, FworkCode = 18, PwayCode = 26 }
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
                    FirstName = "", LastName = new string('*', 101), DateOfBirth = new DateTimeViewModel(8, 12, 1998), TrainingCode = "2", ULN = "1234567890", ProgType = 25,
                    StartDate = new DateTimeViewModel(null, 8, 2120), EndDate = new DateTimeViewModel(null, 12, 2125), Cost = "15000", NINumber = "SE345678A", EmployerRef = "Abba123"
                },
                new ApprenticeshipViewModel
                {
                    FirstName = new string('*', 101), LastName = "", DateOfBirth = new DateTimeViewModel(8, 12, 1998), TrainingCode = "2", ULN = "1234567890", ProgType = 25,
                    StartDate = new DateTimeViewModel(null, 8, 2120), EndDate = new DateTimeViewModel(null, 12, 2125), Cost = "15000", NINumber = "SE345678A", EmployerRef = "Abba123"
                }
            };

            var records = new List<CsvRecord>
            {
                new CsvRecord { ProgType = 25, StdCode = 2 },
                new CsvRecord { ProgType = 25, StdCode = 2 }
            };

            return apprenticeships.Zip(
                records,
                (a, r) => new ApprenticeshipUploadModel { ApprenticeshipViewModel = a, CsvRecord = r });
        }
    }
}
