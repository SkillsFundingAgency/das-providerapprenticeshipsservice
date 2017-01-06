using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    [TestFixture]
    public class WhenValidatingBulkUploadFileAttributes
    {
        private Mock<HttpPostedFileBase> _file;

        private BulkUploadValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes("hello world"));

            _file.Setup(m => m.InputStream).Returns(textStream);

            _sut = new BulkUploadValidator(new ProviderApprenticeshipsServiceConfiguration { MaxBulkUploadFileSize = 512 }, Mock.Of<ILog>());
        }

        [Test]
        public void FileValidationError()
        {
            _file.Setup(m => m.FileName).Returns("abba.csv.txt");
            _file.Setup(m => m.ContentLength).Returns(513 * 1024);

            var errors  = _sut.ValidateFileAttributes(_file.Object).ToList();

            errors.Count.Should().Be(2);
            errors.Count(m => m.ErrorCode.Contains("Filename_01")).Should().Be(1);
            errors[0].Message.Should().Be("Filename must be in the correct format, eg. APPDATA-20161212-201530.csv");
            errors[1].Message.Should().StartWith("File size cannot be larger then");
        }

        [TestCase("APPDATA-20051131-220546.csv", Description = "Date invalid 31 of November")]
        [TestCase("APPDATA-20051115-250001.csv", Description = "Time is invalid, Hour 25")]
        [TestCase("APPDATA-31112005-000000.csv", Description = "Date invalid wrong format")]
        public void FileValidationDateError(string fileName)
        {
            _file.Setup(m => m.FileName).Returns(fileName);

            var errors = _sut.ValidateFileAttributes(_file.Object).ToList();

            errors.Count.Should().Be(1);
            errors.FirstOrDefault().Message.ShouldAllBeEquivalentTo("Filename must be in the correct format, eg. APPDATA-20161212-201530.csv");
            errors.FirstOrDefault().ErrorCode.ShouldAllBeEquivalentTo("Filename_01");
        }

        [TestCase("APPDATA-051131-220546.csv", Description = "Year not comlete")]
        [TestCase("APPDATA-20051115.csv", Description = "Missing time")]
        [TestCase("APPDATA-20051131220526.csv", Description = "Missing - between date and time")]
        public void FileValidationDateStringError(string fileName)
        {
            _file.Setup(m => m.FileName).Returns(fileName);

            var errors = _sut.ValidateFileAttributes(_file.Object).ToList();

            errors.Count.Should().Be(1);
            errors.FirstOrDefault().Message.ShouldAllBeEquivalentTo("Filename must be in the correct format, eg. APPDATA-20161212-201530.csv");
            errors.FirstOrDefault().ErrorCode.ShouldAllBeEquivalentTo("Filename_01");
        }

        [Test(Description = "Date must be in the past")]
        public void FileValidationDateInThePastStringError()
        {
            _file.Setup(m => m.FileName).Returns("APPDATA-21820905-175300.csv");

            var errors = _sut.ValidateFileAttributes(_file.Object).ToList();

            errors.Count.Should().Be(1);
            errors.FirstOrDefault().Message.ShouldAllBeEquivalentTo("The file date/time must be on or before today's date/time");
            errors.FirstOrDefault().ErrorCode.ShouldAllBeEquivalentTo("Filename_02");
        }

        [Test]
        public void FileValidationNoErrors()
        {
            var errors = _sut.ValidateFileAttributes(_file.Object);

            errors.Count().Should().Be(0);
        }
    }
}
