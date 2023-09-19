using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderDetails;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Services
{
    public class TrainingProviderServiceTest
    {
        [Test, MoqAutoData]
        public async Task Then_The_Provider_Is_Found_And_Data_Returned(
            long ukprn,
            GetProviderResponseItem apiResponse,
            [Frozen] Mock<IRecruitApiClient> apiClient,
            [Frozen] Mock<IProviderCommitmentsLogger> logger,
            TrainingProviderService service)
        {
            //Arrange
            var expectedGetUrl = new GetProviderRequest(ukprn);
            apiClient
                .Setup(x => x.Get<GetProviderResponseItem>(
                    It.Is<GetProviderRequest>(c => c.GetUrl.Equals(expectedGetUrl.GetUrl)))).ReturnsAsync(apiResponse);

            //Act
            var actual = await service.GetProviderDetails(ukprn);

            //Assert
            actual.Should().BeEquivalentTo(apiResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_If_NotFound_Response_Then_Null_Returned(
            long ukprn,
            GetProviderResponseItem apiResponse,
            [Frozen] Mock<IRecruitApiClient> apiClient,
            [Frozen] Mock<IProviderCommitmentsLogger> logger,
            TrainingProviderService service)
        {
            //Arrange
            apiClient
                .Setup(x => x.Get<GetProviderResponseItem>(
                    It.IsAny<GetProviderRequest>()))!.ReturnsAsync((GetProviderResponseItem)null);

            //Act
            var actual = await service.GetProviderDetails(ukprn);

            //Assert
            actual.Should().BeNull();
        }
    }
}
