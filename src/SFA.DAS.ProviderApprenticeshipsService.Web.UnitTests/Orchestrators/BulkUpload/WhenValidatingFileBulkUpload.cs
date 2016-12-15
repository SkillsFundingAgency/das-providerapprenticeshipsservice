using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    [TestFixture]
    public class WhenValidatingFileBulkUpload
    {
        private Mock<HttpPostedFileBase> _file;

        private BulkUploader _sut;

        [SetUp]
        public void SetUp()
        {
            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes("hello world"));

            _file.Setup(m => m.InputStream).Returns(textStream);
            _sut = new BulkUploader();
        }

        [Test]
        public void FileValidationError()
        {
            _file.Setup(m => m.FileName).Returns("abba.csv.txt");
            _file.Setup(m => m.ContentLength).Returns(513 * 1000);

            var errors  = _sut.ValidateFile(_file.Object).ToList();

            errors.Count.Should().Be(4);
            errors.Count(m => m.Contains("File name must end with .csv")).Should().Be(1);
            errors.Count(m => m.Contains("File name must start with APPDATA")).Should().Be(1);
            errors.Count(m => m.Contains("File name must include the date with fomat: yyyyMMdd-HHmmss")).Should().Be(1);
            errors.Count(m => m.Contains("File size cannot be larger then ")).Should().Be(1);
        }

        [TestCase("APPDATA-20051131-220546.csv", Description = "Date invalid 31 of November")]
        [TestCase("APPDATA-20051115-250001.csv", Description = "Time is invalid, Hour 25")]
        [TestCase("APPDATA-31112005-000000.csv", Description = "Date invalid wrong format")]
        public void FileValidationDateError(string fileName)
        {
            _file.Setup(m => m.FileName).Returns(fileName);

            var errors = _sut.ValidateFile(_file.Object).ToList();

            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ShouldAllBeEquivalentTo("Date in file name is not valid");
        }

        [TestCase("APPDATA-051131-220546.csv", Description = "Year not comlete")]
        [TestCase("APPDATA-20051115.csv", Description = "Missing time")]
        [TestCase("APPDATA-20051131220526.csv", Description = "Missing - between date and time")]
        public void FileValidationDateStringError(string fileName)
        {
            _file.Setup(m => m.FileName).Returns(fileName);

            var errors = _sut.ValidateFile(_file.Object).ToList();

            errors.Count.Should().Be(1);
            errors.FirstOrDefault().ShouldAllBeEquivalentTo("File name must include the date with fomat: yyyyMMdd-HHmmss");
        }

        [Test]
        public void FileValidationNoErrors()
        {
            var errors = _sut.ValidateFile(_file.Object);

            errors.Count().Should().Be(0);
        }
    }
}
