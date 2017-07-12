﻿using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    [TestFixture]
    public class WhenCreatingRecordsBulkUpload
    {
        const string TestData =
@"CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,NINumber,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,EmpRef,ProviderRef
 Abba123,1113335559,Froberg,Chris,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref
 Abba123,1113335559,Froberg1,Chris1,1998-12-08,SE123321C,25,,,3,2120-08,2125-08,1500,,Employer ref,Provider ref
 ABBA123,1113335559,Froberg2,Chris2,1998-12-08,SE123321C,25,,,3,2120-08,2125-08,1500,,Employer ref,Provider ref
 ABBA123,1113335559,Froberg3,Chris3,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref
 ,,,,,,,,,,,,,,,
 ABBA123,1113335559,Chris3,Froberg3,1998-12-08,SE123321E,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref
 ABBA123,1113335559,Chris2,StartEndDateError,1998-12-08,SE123321C,25,,,2,2120-08,2119-08,1500,,Employer ref,Provider ref
 ABBA123,1113335559,Chris3,Froberg3WrongDateFormat,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref";

        private Mock<HttpPostedFileBase> _file;
        private IBulkUploadFileParser _sut;

        [SetUp]
        public void Setup()
        {
            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(TestData));
            _file.Setup(m => m.InputStream).Returns(textStream);

            _sut = new BulkUploadFileParser(Mock.Of<IProviderCommitmentsLogger>());
        }

        [Test]
        public void CreatingViewModels()
        {
            // ToDo: Move this to where we do the reading of file.
            //var records = _sut.CreateViewModels(_file.Object);
            var records = _sut.CreateViewModels(123, 456, TestData);
            records.Data.Count().Should().Be(8);
            records.Errors.Should().NotBeNull();
            records.Errors.Should().BeEmpty();
        }

        [Test]
        public void WhenFileIsEmpty()
        {
            var records = _sut.CreateViewModels(123, 456, "");
            records.Data.Should().BeNull();
            records.Errors.Should().NotBeNull();
            records.Errors.First().Message.Should().Be("Upload failed. Please check your file and try again.");
        }
    }
}
