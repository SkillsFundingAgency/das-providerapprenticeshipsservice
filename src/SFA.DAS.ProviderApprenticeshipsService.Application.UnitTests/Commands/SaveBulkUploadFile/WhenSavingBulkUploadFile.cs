using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SaveBulkUploadFile;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SaveBulkUploadFile
{
    [TestFixture]
    public class WhenSavingBulkUploadFile
    {
        private Mock<IProviderCommitmentsApi> _mockApi; 
        private Mock<IValidator<SaveBulkUploadFileCommand>> _mockValidator;
        private SaveBulkUploadFileHandler _sut;
        private SaveBulkUploadFileCommand _command;

        [SetUp]
        public void SetUp()
        {
            _mockApi = new Mock<IProviderCommitmentsApi>();
            _mockValidator = new Mock<IValidator<SaveBulkUploadFileCommand>>();
            _sut = new SaveBulkUploadFileHandler(_mockApi.Object, _mockValidator.Object);

            _command =
                new SaveBulkUploadFileCommand
                {
                    ProviderId = 666,
                    CommitmentId = 1,
                    FileName = "Abba.csv",
                    FileContent = "content"
                };
            _mockValidator.Setup(m => m.Validate(_command)).Returns(new ValidationResult());
            _mockApi.Setup(m => m.BulkUploadFile(_command.ProviderId, It.IsAny<BulkUploadFileRequest>()))
                .ReturnsAsync(1882);
        }

        [Test]
        public async Task ShouldValidateAndCallApi()
        {
            var result = await _sut.Handle(_command);

            _mockValidator.Verify(m => m.Validate(_command), Times.Once);
            _mockApi.Verify(m => m.BulkUploadFile(_command.ProviderId, It.IsAny<BulkUploadFileRequest>()), Times.Once);

            result.Should().Be(1882);
        }

        [Test]
        public void ShouldFailValidation()
        {
            _mockValidator.Setup(m => m.Validate(_command))
                .Returns(
                    new ValidationResult(new[] { new ValidationFailure("FailingPropery", "Failing error message"), }));

            Func<Task> act = async () => await _sut.Handle(_command);
            act.ShouldThrow<ValidationException>();

            _mockValidator.Verify(m => m.Validate(_command), Times.Once);
            _mockApi.Verify(m => m.BulkUploadFile(_command.ProviderId, It.IsAny<BulkUploadFileRequest>()), Times.Never);
        }
    }
}

