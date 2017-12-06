using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    [TestFixture]
    public class WhenCreatingRecordsBulkUpload
    {
        private static List<string> headers = new List<string>
        {
            "CohortRef",
            "ULN",
            "FamilyName",
            "GivenNames",
            "DateOfBirth",
            "NINumber",
            "ProgType",
            "FworkCode",
            "PwayCode",
            "StdCode",
            "StartDate",
            "EndDate",
            "TotalPrice",
            "EPAOrgId",
            "EmpRef",
            "ProviderRef"
        };

        private readonly string _testData;

        private Mock<HttpPostedFileBase> _file;

        private IBulkUploadFileParser _sut;

        public WhenCreatingRecordsBulkUpload()
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",", headers));
            builder.Append(@"Abba123,1113335559,Froberg,Chris,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref
 Abba123,1113335559,Froberg1,Chris1,1998-12-08,SE123321C,25,,,3,2120-08,2125-08,1500,,Employer ref,Provider ref
 ABBA123,1113335559,Froberg2,Chris2,1998-12-08,SE123321C,25,,,3,2120-08,2125-08,1500,,Employer ref,Provider ref
 ABBA123,1113335559,Froberg3,Chris3,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref
 ,,,,,,,,,,,,,,,
 ABBA123,1113335559,Chris3,Froberg3,1998-12-08,SE123321E,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref
 ABBA123,1113335559,Chris2,StartEndDateError,1998-12-08,SE123321C,25,,,2,2120-08,2119-08,1500,,Employer ref,Provider ref
 ABBA123,1113335559,Chris3,Froberg3WrongDateFormat,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref");

            _testData = builder.ToString();
        }

        [SetUp]
        public void Setup()
        {
           
            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(_testData));
            _file.Setup(m => m.InputStream).Returns(textStream);

            _sut = new BulkUploadFileParser(Mock.Of<IProviderCommitmentsLogger>());
        }

        [Test]
        public void CreatingViewModels()
        {
            // ToDo: Move this to where we do the reading of file.
            //var records = _sut.CreateViewModels(_file.Object);
            var records = _sut.CreateViewModels(123, 456, _testData);
            records.Data.Count().Should().Be(8);
            records.Errors.Should().NotBeNull();
            records.Errors.Should().BeEmpty();
        }

        [Test]
        public void WhenFileIsEmpty()
        {
            var logger = new Mock<IProviderCommitmentsLogger>();
            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>())).Verifiable();
            logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>())).Verifiable();
            _sut = new BulkUploadFileParser(logger.Object);

            var records = _sut.CreateViewModels(123, 456, "");
            records.Data.Should().BeNull();
            records.Errors.Should().NotBeNull();
            records.Errors.First().Message.Should().Be("Upload failed. Please check your file and try again.");

            logger.Verify(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Never);
            logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Once);

        }

        [Test]
        public void ThenShouldLogInfoWhenNoHeaderRecord()
        {
            var logger = new Mock<IProviderCommitmentsLogger>();
            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>())).Verifiable();
            logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>())).Verifiable();
            _sut = new BulkUploadFileParser(logger.Object);

            var builder = new StringBuilder();
            builder.AppendLine(Environment.NewLine);
            builder.AppendLine("Abba123,1113335559,Froberg,Chris,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref");
            var inputData = builder.ToString();

            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(inputData.Length);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(inputData));
            _file.Setup(m => m.InputStream).Returns(textStream);

            var result = _sut.CreateViewModels(123, 456, inputData);

            var errors = result.Errors.ToList();
            Assert.AreEqual(0, errors.Count);
            Assert.AreEqual(0, result.Data.Count());

            logger.Verify(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Never);
            logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Never);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidColumnHeaders))]
        public void ThenShouldLogInfoForMissingFields(string header)
        {
            var logger = new Mock<IProviderCommitmentsLogger>();
            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>())).Verifiable();
            logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>())).Verifiable();
            _sut = new BulkUploadFileParser(logger.Object);

            var inputData = $"{header}" +
                            @"
                            Abba123,1113335559,Froberg,Chris,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref";

            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(inputData.Length);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(inputData));
            _file.Setup(m => m.InputStream).Returns(textStream);

           var result = _sut.CreateViewModels(123, 456, inputData);

            var errors = result.Errors.ToList();
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Some mandatory fields are incomplete. Please check your file and upload again.", errors.First().Message);

            logger.Verify(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Once);
            logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Never);
        }

        private static IEnumerable<string> GetInvalidColumnHeaders()
        {
            yield return "ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId";
        }
    }
}
