using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetBulkUploadFile;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetBulkUploadFile
{
    [TestFixture]
    public class WhenIGetBulkUploadFile
    {
        private GetBulkUploadFileQueryHandler _handler;
        private Mock<IProviderCommitmentsApi> _providerCommitmentsApi;

        [SetUp]
        public void Arrange()
        {
            _providerCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _providerCommitmentsApi.Setup(x => x.BulkUploadFile(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync("TEST");

            _handler = new GetBulkUploadFileQueryHandler(_providerCommitmentsApi.Object);
        }

        [Test]
        public async Task TheTheApiIsCalledToRetrieveFileContent()
        {
            //Arrange
            var command = new GetBulkUploadFileQueryRequest
            {
                BulkUploadId = 1,
                ProviderId = 2
            };

            //Act
            await _handler.Handle(command, new CancellationToken());

            //Assert
            _providerCommitmentsApi.Verify(x =>
                    x.BulkUploadFile(It.IsAny<long>(),
                    It.IsAny<long>()),
                Times.Once);
        }

        [Test]
        public async Task ThenTheDataRetrievedIsMappedToResponse()
        {
            //Arrange
            var command = new GetBulkUploadFileQueryRequest
            {
                BulkUploadId = 1,
                ProviderId = 2
            };

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            Assert.AreEqual("TEST", result.FileContent);
        }
    }
}
