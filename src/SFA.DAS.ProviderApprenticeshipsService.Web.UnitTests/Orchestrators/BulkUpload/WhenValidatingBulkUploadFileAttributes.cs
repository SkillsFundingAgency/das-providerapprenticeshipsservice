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
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

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
            _sut = BulkUploadTestHelper.GetBulkUploadValidator(512);
         
        }

        [Test]
        public void FileValidationNoErrors()
        {
            var errors = _sut.ValidateFileSize(_file.Object);

            errors.Count().Should().Be(0);
        }

        [Test]
        public void FileTooBig()
        {
            _file.Setup(m => m.ContentLength).Returns(2000000);
            var errors = _sut.ValidateFileSize(_file.Object);

            errors.Count().Should().Be(1);
            errors.First().Message.Should().Be("File size cannot be larger than 512 kb");
        }
    }
}
