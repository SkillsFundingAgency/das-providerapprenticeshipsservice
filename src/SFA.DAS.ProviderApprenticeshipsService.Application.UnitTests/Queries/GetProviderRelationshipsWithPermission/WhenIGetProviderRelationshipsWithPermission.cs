using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types;

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
                Permission = PermissionEnumDto.CreateCohort,
                ProviderId = 12345
            };

            _apiClient = new Mock<IProviderRelationshipsApiClient>();
            _apiClient.Setup(x => x.ListRelationshipsWithPermission(It.IsAny<ProviderRelationshipRequest>()))
                .ReturnsAsync(() => new ProviderRelationshipResponse
                {
                    ProviderRelationships = new List<ProviderRelationshipResponse.ProviderRelationship>()
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
                x.ListRelationshipsWithPermission(
                    It.Is<ProviderRelationshipRequest>(r => r.Ukprn == _validRequest.ProviderId)));
        }

        [Test]
        public async Task TheApiClientIsCalledWithTheRequestedPermission()
        {
            //Act
            await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            _apiClient.Verify(x =>
                x.ListRelationshipsWithPermission(
                    It.Is<ProviderRelationshipRequest>(r => r.Permission == _validRequest.Permission)));
        }

        [Test]
        public async Task TheResultIsReturnedFromTheRelationshipsApiClient()
        {
            //Arrange
            var apiResponse = new ProviderRelationshipResponse()
            {
                ProviderRelationships = new List<ProviderRelationshipResponse.ProviderRelationship>()
            };

            _apiClient.Setup(x => x.ListRelationshipsWithPermission(It.IsAny<ProviderRelationshipRequest>()))
                .ReturnsAsync(apiResponse);

            //Act
            var result = await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            Assert.AreEqual(result.ProviderRelationships, apiResponse.ProviderRelationships);
        }
    }
}
