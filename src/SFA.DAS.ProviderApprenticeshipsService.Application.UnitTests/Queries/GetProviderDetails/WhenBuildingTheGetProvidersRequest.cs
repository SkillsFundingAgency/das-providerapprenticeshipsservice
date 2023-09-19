using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderDetails;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetProviderDetails
{
    public class WhenBuildingTheGetProvidersRequest
    {
        [Test, MoqAutoData]
        public void Then_The_Url_Is_Correctly_Built(long ukprn)
        {
            //Arrange
            var actual = new GetProviderRequest(ukprn);

            //Assert
            actual.GetUrl.Should().Be($"providers/{ukprn}");
        }
    }
}
