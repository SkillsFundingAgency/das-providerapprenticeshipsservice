using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    [TestFixture]
    public class WhenCreatingRecordsBulkUpload
    {
        const string TestData =
            @"CohortRef,GivenNames,FamilyName,DateOfBirth,NINumber,FworkCode,PwayCode,ProgType,StdCode,LearnStartDate,LearnPlanEndDate,TrainingPrice,EPAPrice,EPAOrgId,EmpRef,ProvRef,ULN
                Abba123,Chris,Froberg,1998-12-08,SE1233211C,,,,2,2020-08-01,2025-08-01,1500,,,Employer ref,Provider ref,1113335559
                Abba123,Chris1,Froberg1,1998-12-08,SE1233211C,,,,3,2020-08-01,2025-08-01,1500,,,Employer ref,Provider ref,1113335559
                Abba123,Chris2,Froberg2,1998-12-08,SE1233211C,,,,4,2020-08-01,2025-08-01,1500,,,Employer ref,Provider ref,1113335559
                Abba123,Chris3,Froberg3,1998-12-08,SE1233211C,,,,5,2020-08-01,2025-08-01,1500,,,Employer ref,Provider ref,1113335559
                ,,,,,,,,,,,,,,,,
                Abba123,Chris3,Froberg3,1998-12-08,SE1233211E,,,,5,2020-08-01,2025-08-01,1500,,,Employer ref,Provider ref,1113335559
                Abba123,Chris2,StartEndDateError,1998-12-08,SE1233211C,,,,4,2020-08-01,2019-08-01,1500,,,Employer ref,Provider ref,1113335559
                Abba123,Chris3,Froberg3WrongDateFormat,1998-12-08,SE1233211C,,,,5,2020-08-01,01/08/25,1500,,,Employer ref,Provider ref,1113335559";

        private Mock<HttpPostedFileBase> _file;

        BulkUploader _sut;

        [SetUp]
        public void Setup()
        {
            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(TestData));
            _file.Setup(m => m.InputStream).Returns(textStream);

            _sut = new BulkUploader();
        }

        [Test]
        public void CreatingViewModels()
        {
            var records = _sut.CreateViewModels(_file.Object);
            records.Count().Should().Be(8);
        }

        [Test]
        public void ValidatingViewModels()
        {
            var records = _sut.CreateViewModels(_file.Object);
            var result = _sut.ValidateFields(records, new List<ITrainingProgramme>());
            result.Count().Should().Be(58);
            result.Select(m => m.ToString())
                .Contains("Row:8 - The Learning planned end date must be entered and be in the format yyyy-mm-dd")
                .Should()
                .BeTrue();
        }
    }
}
