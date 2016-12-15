using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

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
            var errors = _sut.ValidateFields(new ApprenticeshipViewModel[0], TrainingProgrammes()).ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ShouldBeEquivalentTo("File contains no records");
        }

        [Test]
        public void MissingTraingCode()
        {
            var errors = _sut.ValidateFields(GetTestData(), new List<ITrainingProgramme>()).ToList();
            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ShouldBeEquivalentTo("Not a valid training code: 2");
        }

        [Test]
        public void FailingValidationOnName()
        {
            var errors = _sut.ValidateFields(GetFailingTestData(), TrainingProgrammes()).ToList();
            errors.Count.Should().Be(4);
            errors[0].ShouldBeEquivalentTo("Row:1 - Enter a first name");
            errors[1].ShouldBeEquivalentTo("Row:1 - Last name cannot contain more then 100 chatacters");
            errors[2].ShouldBeEquivalentTo("Row:2 - First name cannot contain more then 100 chatacters");
            errors[3].ShouldBeEquivalentTo("Row:2 - Enter a last name");
        }

        private List<ITrainingProgramme> TrainingProgrammes()
        {
            return new List<ITrainingProgramme>
                       {
                            new Framework { FrameworkName = "Framework1", Id = "1-2-3" },
                            new Framework { FrameworkName = "Framework2", Id = "`4-5-6" },
                            new Standard { Title = "Standard 1", Id = "1" },
                            new Standard { Title = "Standard 2", Id = "2" }
                       };
        }

        private List<ApprenticeshipViewModel> GetTestData()
        {
            return new List<ApprenticeshipViewModel>
            {
                new ApprenticeshipViewModel
                {
                    FirstName = "Bob", LastName = "The cat", DateOfBirthYear = 1998, DateOfBirthMonth = 12, DateOfBirthDay = 08, TrainingCode = "2", ULN = "1234567890",
                    StartMonth = 8, StartYear = 2120, EndMonth = 12, EndYear = 2125, Cost = "15000", NINumber = "SE345678A", EmployerRef = "Abba123"
                }
            };
        }

        private List<ApprenticeshipViewModel> GetFailingTestData()
        {
            return new List<ApprenticeshipViewModel>
            {
                new ApprenticeshipViewModel
                {
                    FirstName = "", LastName = new string('*', 101), DateOfBirthYear = 1998, DateOfBirthMonth = 12, DateOfBirthDay = 08, TrainingCode = "2", ULN = "1234567890",
                    StartMonth = 8, StartYear = 2120, EndMonth = 12, EndYear = 2125, Cost = "15000", NINumber = "SE345678A", EmployerRef = "Abba123"
                },
                new ApprenticeshipViewModel
                {
                    FirstName = new string('*', 101), LastName = "", DateOfBirthYear = 1998, DateOfBirthMonth = 12, DateOfBirthDay = 08, TrainingCode = "2", ULN = "1234567890",
                    StartMonth = 8, StartYear = 2120, EndMonth = 12, EndYear = 2125, Cost = "15000", NINumber = "SE345678A", EmployerRef = "Abba123"
                }
            };
        }
    }
}
