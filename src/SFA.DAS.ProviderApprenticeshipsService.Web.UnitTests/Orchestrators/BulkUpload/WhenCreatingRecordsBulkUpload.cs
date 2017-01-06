using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    [TestFixture]
    public class WhenCreatingRecordsBulkUpload
    {
        const string TestData =
@"CohortRef,GivenNames,FamilyName,DateOfBirth,NINumber,FworkCode,PwayCode,ProgType,StdCode,LearnStartDate,LearnPlanEndDate,TrainingPrice,EPAPrice,EPAOrgId,EmpRef,ProvRef,ULN
 Abba123,Chris,Froberg,1998-12-08,SE123321C,,,25,2,2120-08-01,2125-08-01,1500,,,Employer ref,Provider ref,1113335559
 Abba123,Chris1,Froberg1,1998-12-08,SE123321C,,,25,3,2120-08-01,2125-08-01,1500,,,Employer ref,Provider ref,1113335559
 ABBA123,Chris2,Froberg2,1998-12-08,SE123321C,,,25,3,2120-08-01,2125-08-01,1500,,,Employer ref,Provider ref,1113335559
 ABBA123,Chris3,Froberg3,1998-12-08,SE123321C,,,25,2,2120-08-01,2125-08-01,1500,,,Employer ref,Provider ref,1113335559
 ,,,,,,,,,,,,,,,,
 ABBA123,Chris3,Froberg3,1998-12-08,SE123321E,,,25,2,2120-08-01,2125-08-01,1500,,,Employer ref,Provider ref,1113335559
 ABBA123,Chris2,StartEndDateError,1998-12-08,SE123321C,,,25,2,2120-08-01,2119-08-01,1500,,,Employer ref,Provider ref,1113335559
 ABBA123,Chris3,Froberg3WrongDateFormat,1998-12-08,SE123321C,,,25,2,2120-08-01,2125-08-01,1500,,,Employer ref,Provider ref,1113335559";

        private Mock<HttpPostedFileBase> _file;

        BulkUploadValidator _sut;

        [SetUp]
        public void Setup()
        {
            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(TestData));
            _file.Setup(m => m.InputStream).Returns(textStream);

            _sut = new BulkUploadValidator();
        }

        [Test]
        public void CreatingViewModels()
        {
            var records = _sut.CreateViewModels(_file.Object);
            records.Data.Count().Should().Be(8);
            records.Errors.Should().NotBeNull();
            records.Errors.Should().BeEmpty();
        }

        [Test]
        public void ValidatingViewModels()
        {
            var records = _sut.CreateViewModels(_file.Object);
            var result = _sut.ValidateFields(records.Data, new List<ITrainingProgramme>{ new Standard {Id = "2"}, new Standard { Id = "3" } }, "ABBA123");
            result.Count().Should().Be(20);
            var resultMessages = result.Select(m => m.ToString());

            resultMessages.Count(m => m.StartsWith("Row:5 ")).Should().Be(16);
            resultMessages.Count(m => m.Equals("Row:7 - The Learning planned end date must not be on or before the Learning start date")).Should().Be(1);
        }
    }
}
