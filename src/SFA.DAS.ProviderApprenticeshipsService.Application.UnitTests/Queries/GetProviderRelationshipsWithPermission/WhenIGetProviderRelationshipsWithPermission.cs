using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetProviderRelationshipsWithPermission
{
    [TestFixture]
    public class WhenIGetProviderRelationshipsWithPermission
    {
        private GetProviderRelationshipsWithPermissionQueryHandler _handler;
        private Mock<IProviderRelationshipsApiClient> _apiClient;
        private GetProviderRelationshipsWithPermissionQueryRequest _validRequest;

        [SetUp]
        public void Arrange()
        {
            _validRequest = new GetProviderRelationshipsWithPermissionQueryRequest
            {
                Permission = Operation.CreateCohort,
                ProviderId = 12345
            };

            _apiClient = new Mock<IProviderRelationshipsApiClient>();
            _apiClient.Setup(x => x.GetAccountProviderLegalEntitiesWithPermission(It.IsAny<GetAccountProviderLegalEntitiesWithPermissionRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetAccountProviderLegalEntitiesWithPermissionResponse
                {
                    AccountProviderLegalEntities = new List<AccountProviderLegalEntityDto>()
                });


            _handler = new GetProviderRelationshipsWithPermissionQueryHandler(_apiClient.Object);
        }

        [Test]
        public async Task TheApiClientIsCalledWithTheProviderId()
        {
            //Act
            await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            _apiClient.Verify(x =>
                x.GetAccountProviderLegalEntitiesWithPermission(
                    It.Is<GetAccountProviderLegalEntitiesWithPermissionRequest>(r => r.Ukprn == _validRequest.ProviderId), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task TheApiClientIsCalledWithTheRequestedPermission()
        {
            //Act
            await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            _apiClient.Verify(x =>
                x.GetAccountProviderLegalEntitiesWithPermission(
                    It.Is<GetAccountProviderLegalEntitiesWithPermissionRequest>(r => r.Operation == _validRequest.Permission), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task TheResultIsReturnedFromTheRelationshipsApiClient()
        {
            //Arrange
            var apiResponse = new GetAccountProviderLegalEntitiesWithPermissionResponse
            {
                AccountProviderLegalEntities = new List<AccountProviderLegalEntityDto>()
            };

            _apiClient.Setup(x => x.GetAccountProviderLegalEntitiesWithPermission(It.IsAny<GetAccountProviderLegalEntitiesWithPermissionRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            //Act
            var result = await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            Assert.AreEqual(result.ProviderRelationships, apiResponse.AccountProviderLegalEntities);
        }
    }
}
