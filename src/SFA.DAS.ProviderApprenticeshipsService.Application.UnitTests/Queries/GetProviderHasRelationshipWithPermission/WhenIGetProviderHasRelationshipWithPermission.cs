using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetProviderHasRelationshipWithPermission
{
    [TestFixture]
    public class WhenIGetProviderHasRelationshipWithPermission
    {
        private GetProviderHasRelationshipWithPermissionQueryHandler _handler;
        private Mock<IProviderRelationshipsApiClient> _apiClient;
        private GetProviderHasRelationshipWithPermissionQueryRequest _validRequest;

        [SetUp]
        public void Arrange()
        {
            _validRequest = new GetProviderHasRelationshipWithPermissionQueryRequest
            {
                Permission = Operation.CreateCohort,
                ProviderId = 12345
            };

            _apiClient = new Mock<IProviderRelationshipsApiClient>();
            _apiClient.Setup(x => x.HasRelationshipWithPermission(It.IsAny<RelationshipsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _handler = new GetProviderHasRelationshipWithPermissionQueryHandler(_apiClient.Object);
        }

        [Test]
        public async Task TheApiClientIsCalledWithTheProviderId()
        {
            //Act
            await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            _apiClient.Verify(x =>
                x.HasRelationshipWithPermission(
                    It.Is<RelationshipsRequest>(r => r.Ukprn == _validRequest.ProviderId),It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task TheApiClientIsCalledWithTheRequestedPermission()
        {
            //Act
            await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            _apiClient.Verify(x =>
                x.HasRelationshipWithPermission(
                    It.Is<RelationshipsRequest>(r => r.Operation == _validRequest.Permission), It.IsAny<CancellationToken>()));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task TheResultIsReturnedFromTheRelationshipsApiClient(bool apiResponse, bool expectResult)
        {
            //Arrange
            _apiClient.Setup(x => x.HasRelationshipWithPermission(It.IsAny<RelationshipsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            //Act
            var result = await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            Assert.AreEqual(result.HasPermission, expectResult);
        }

    }
}
